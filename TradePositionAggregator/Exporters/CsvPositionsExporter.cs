using Microsoft.Extensions.Logging;
using PetroineosAggregatedVolume.Interfaces;
using PetroineosAggregatedVolume.Models;
using System.Text;

namespace PetroineosAggregatedVolume.Exporters
{
    public class CsvPositionsExporter : IExporter
    {
        ILogger<CsvPositionsExporter> _logger;

        public CsvPositionsExporter(ILogger<CsvPositionsExporter> logger)
        {
            _logger = logger;
        }
        public string ExportPositions(DateTime date, AggregatedPosition positions)
        {
            _logger.LogInformation($"Exporting positions for {date.ToString("yyyy-MM-dd HH:mm")}");

            var sb = new StringBuilder();

            sb.AppendLine("Local Time,Volume");

            foreach (var p in positions.Periods)
            {
                sb.AppendLine($"{PeriodToLocalTime(date, p.Period)},{p.Volume}");
            }

            return sb.ToString();   
        }

        private string PeriodToLocalTime(DateTime localDate, int period)
        {
            // start at 23:00 on the previous date
            var prevDate = localDate.Date.AddHours(-1); 
            
            // use DateTimeOffset to account for daylight savings short and long dates
            var dto = new DateTimeOffset(prevDate).AddHours(period - 1); 

            return dto.ToLocalTime().ToString("HH:mm");
        }
    }
}
