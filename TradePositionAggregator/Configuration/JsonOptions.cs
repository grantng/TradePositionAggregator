using System.Text.Json;

namespace PetroineosAggregatedVolume.Configuration
{
    public class JsonOptions
    {
        public static readonly JsonSerializerOptions Default = new()
        {
            WriteIndented = true
        };
    }
}
