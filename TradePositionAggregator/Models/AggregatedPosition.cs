using TradePositionAggregator.Configuration;
using Services;
using System.Text.Json;

namespace TradePositionAggregator.Models
{
    public class AggregatedPosition
    {
        public List<PowerPeriod> Periods { get; set; }

        public AggregatedPosition(IEnumerable<PowerPeriod> periods)
        {
            Periods = periods.ToList();
        }

        public override string ToString()
        {
            var json = JsonSerializer.Serialize(Periods, JsonOptions.Default);

            return json;
        }
    }
}
