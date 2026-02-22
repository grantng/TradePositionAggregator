using Microsoft.Extensions.Hosting;
using TradePositionAggregator.Configuration;
using Microsoft.Extensions.Configuration;
using TradePositionAggregator.Extensions;
using Serilog;

namespace TradePositionAggregator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CreateLogger();

            try
            {
                var builder = CreateHostBuilder(args);

                // Detect if running as Windows Service
                if (!Environment.UserInteractive)
                {
                    builder.UseWindowsService();
                }

                var host = builder.Build();
                Log.Information("Starting host");
                host.Run();
            }
            catch (Exception ex)
            {
                // Log fatal startup errors
                Log.Fatal(ex, "Host terminated unexpectedly");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
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
            try
            {
                //read appsettings.json directly for log directory and use default if missing
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                    .AddEnvironmentVariables()
                    .Build();

                var logDirectory = config["AppSettings:LogDirectory"];
                if (string.IsNullOrWhiteSpace(logDirectory))
                {
                    logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
                }

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
            catch
            {
                // If logger setup fails, fall back to console-only logger to ensure some logging is available
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .CreateLogger();
            }
        }
    }
}
