using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PetroineosAggregatedVolume.Configuration;
using PetroineosAggregatedVolume.Interfaces;
using Polly;
using Polly.Retry;
using Services;
using TradePositionAggregator.Interfaces;

namespace PetroineosAggregatedVolume
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private ITradeRepository _tradeRepository;
        private IVolumeCalculator _volumeCalculator;
        private IFileWriter _fileWriter;
        private IExporter _exporter;
        private AppSettings _appSettings;
        private AsyncRetryPolicy _retryPolicy;

        public Worker(ILogger<Worker> logger, ITradeRepository tradeRepository, IVolumeCalculator volumeCalculator, IExporter exporter, IFileWriter fileWriter)
        {
            _logger = logger;
            _tradeRepository = tradeRepository;
            _exporter = exporter;
            _fileWriter = fileWriter;
            _volumeCalculator = volumeCalculator;
            _appSettings = AppSettings.GetAppSettings();

            //simple retry policy: 3 attempts with 10 second delay
            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(10));
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            _logger.LogInformation("Service started");
            _logger.LogInformation("AppSettings: {Settings}", _appSettings.ToString());

            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(_appSettings.IntervalMinutes));

            await RunJobAsync(token);

            try
            {
                while (await timer.WaitForNextTickAsync(token))
                {
                    await RunJobAsync(token);
                }
            }
            catch (OperationCanceledException)
            {
                // Service cancelled
            }
        }

        private async Task RunJobAsync(CancellationToken token)
        {
            if (!await _lock.WaitAsync(0, token))
                return; // Skip if already running

            var now = DateTime.Now;

            try
            {
                _logger.LogInformation("Aggregating trade positions for {Time}", now.ToString("yyyy-MM-dd HH:mm"));

                var trades = await GetTrades(now);

                var aggregatedPosition = _volumeCalculator.AggregateTrades(trades);

                var contents = _exporter.ExportPositions(now, aggregatedPosition);

                _fileWriter.WriteToFile(now, contents);

                _logger.LogInformation("Completed aggregating trade positions for {Time}", now.ToString("yyyy-MM-dd HH:mm"));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to aggregate positions for {Time}.\nDetails: {Message}", now.ToString("yyyy-MM-dd HH:mm"), e.Message);
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task<IEnumerable<PowerTrade>> GetTrades(DateTime date)
        {
            try
            {
                var trades = await _retryPolicy.ExecuteAsync(() => _tradeRepository.GetTradesAsync(date));

                return trades;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to retrieve trades for {Time}", date.ToString("yyyy-MM-dd HH:mm"));
                throw;
            }
        }

        public override void Dispose()
        {
            _lock?.Dispose();
            base.Dispose();
        }
    }
}
