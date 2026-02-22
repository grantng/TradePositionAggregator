using Microsoft.Extensions.Logging;
using TradePositionAggregator.Interfaces;
using TradePositionAggregator.Models;
using Services;

namespace TradePositionAggregator
{
    public class VolumeCalculator : IVolumeCalculator
    {
        ILogger<VolumeCalculator> _logger;

        public VolumeCalculator(ILogger<VolumeCalculator> logger)
        {
            _logger = logger;
        }

        public AggregatedPosition AggregateTrades(IEnumerable<PowerTrade> trades)
        {
            if (trades is null)
            {
                throw new ArgumentNullException(nameof(trades));
            }

            var tradeList = trades.ToList();

            var count = tradeList.Count;

            _logger.LogInformation("Aggregating volumes for {Count} trades", count);

            try
            {
                var totals = new Dictionary<int, double>();

                //Create a key for every unique period over all trades
                foreach (var t in tradeList)
                {
                    //sum by each period
                    foreach (var p in t.Periods)
                    {
                        if (totals.ContainsKey(p.Period))
                        {
                            totals[p.Period] += p.Volume;
                        }
                        else
                        {
                            totals[p.Period] = p.Volume;
                        }
                    }
                }

                var keys = totals.Keys.OrderBy(x => x);
                var periods = new List<PowerPeriod>();

                //create a PowerPeriod for each aggregated volume
                foreach (var p in keys)
                {
                    var period = new PowerPeriod()
                    {
                        Period = p,
                        Volume = totals[p]
                    };

                    periods.Add(period);
                }

                _logger.LogInformation("Aggregating volumes for {Count} trades completed", count);

                return new AggregatedPosition(periods);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to aggregate trades. Details: {Message}", e.Message);

                return new AggregatedPosition(new List<PowerPeriod>());
            }
        }
    }
}
