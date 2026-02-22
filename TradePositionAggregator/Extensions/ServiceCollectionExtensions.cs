using Microsoft.Extensions.DependencyInjection;
using TradePositionAggregator.Exporters;
using TradePositionAggregator.Interfaces;

namespace TradePositionAggregator.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddHostedService<Worker>();
            services.AddSingleton<IExporter, CsvPositionsExporter>();
            services.AddSingleton<IFileWriter, FileWriter>();
            services.AddSingleton<IVolumeCalculator, VolumeCalculator>();
            services.AddSingleton<ITradeRepository, TradeRepository>();
            return services;
        }
    }
}