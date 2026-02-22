using System.Text.Json;

namespace TradePositionAggregator.Configuration
{
    public class JsonOptions
    {
        public static readonly JsonSerializerOptions Default = new()
        {
            WriteIndented = true
        };
    }
}
