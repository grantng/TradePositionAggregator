using Microsoft.Extensions.Logging;
using TradePositionAggregator.Configuration;
using TradePositionAggregator.Interfaces;

namespace TradePositionAggregator
{
    public class FileWriter : IFileWriter
    {
        private ILogger<FileWriter> _logger;

        public FileWriter(ILogger<FileWriter> logger)
        {
            _logger = logger;
        }

        public async Task WriteToFileAsync(DateTime time, string contents)
        {
            try
            {
                Directory.CreateDirectory(AppSettings.GetAppSettings().OutputDirectory);
                var path = GetPath(time);

                _logger.LogInformation("Writing file: {Path}", path);

                await File.WriteAllTextAsync(path, contents);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to write to file for: {Time}\nDetails: {Message}", time.ToString("yyyy-MM-dd HH:mm"), e.Message);
                throw;
            }
        }

        private string GetFilename(DateTime time)
        {
            return $"PowerPosition_{time.ToString("yyyyMMdd_HHmm")}.csv";
        }

        private string GetPath(DateTime time)
        {
            var filename = GetFilename(time);
            var folder = AppSettings.GetAppSettings().OutputDirectory;
            var path = Path.Join(folder, filename);

            return path;
        }
    }
}
