using Microsoft.Extensions.Logging;
using PetroineosAggregatedVolume.Interfaces;
using PetroineosAggregatedVolume.Models;
using Services;
using System.Text.Json;

namespace PetroineosAggregatedVolume
{
    public class VolumeCalculator : IVolumeCalculator
    {
        ILogger<VolumeCalculator> _logger;

        public VolumeCalculator(ILogger<VolumeCalculator> logger)
        {
            _logger = logger;

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
        }

        public AggregatedPosition AggregateTrades(IEnumerable<PowerTrade> trades)
        {
            _logger.LogInformation($"Aggregating volumes for {trades.Count()} trades");
            try
            {
                var totals = new Dictionary<int, double>();

                //Create a key for every unique period over all trades
                foreach (var t in trades)
                {
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

                //sum by each period
                foreach (var p in keys)
                {
                    var period = new PowerPeriod()
                    {
                        Period = p,
                        Volume = totals[p]
                    };

                    periods.Add(period);
                }

                _logger.LogInformation($"Aggregating volumes for {trades.Count()} trades completed");


                return new AggregatedPosition(periods);
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to aggregate trades. Details: {e.Message}");
                return new AggregatedPosition(new List<PowerPeriod>());
            }
        }
    }
}
