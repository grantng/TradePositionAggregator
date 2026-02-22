using Microsoft.Extensions.Hosting;
using PetroineosAggregatedVolume.Configuration;
using PetroineosAggregatedVolume.Extensions;
using Serilog;

namespace TradePositionAggregator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CreateLogger();
            var builder = CreateHostBuilder(args);

            // Detect if running as Windows Service
            if (!Environment.UserInteractive)
            {
                builder.UseWindowsService();
            }

            var host = builder.Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices(services =>
                {
                    services.AddApplicationServices();
                });
        }

        private static void CreateLogger()
        {
            var logDirectory = AppSettings.GetAppSettings().LogDirectory;
            Directory.CreateDirectory(logDirectory);
            var path = Path.Combine(logDirectory, "log-.txt");

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File(
                path: path,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30)
            .CreateLogger();
        }
    }
}
