//using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace TradePositionAggregator.Configuration
{
    public class AppSettings
    {
        private static AppSettings _appSettings;
        public string OutputDirectory { get; set; }
        public int IntervalMinutes { get; set; }
        public string LogDirectory { get; set; }

        public static AppSettings GetAppSettings()
        {
            if (_appSettings == null)
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build();

                _appSettings = config
                    .GetSection("AppSettings")
                    .Get<AppSettings>() ?? new AppSettings();

                // Ensure IntervalMinutes > 0 otherwise default to 1
                if (_appSettings.IntervalMinutes <= 0)
                {
                    _appSettings.IntervalMinutes = 1;
                }
            }

            return _appSettings;
        }

        public override string ToString()
        {
            return $"OutputDirectory: {OutputDirectory}\n"
                + $"LogDirectory: {LogDirectory}\n"
                + $"IntervalMinutes: {IntervalMinutes}";
        }
    }
}
