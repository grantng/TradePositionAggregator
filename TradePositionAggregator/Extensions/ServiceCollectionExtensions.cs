using Microsoft.Extensions.DependencyInjection;
using PetroineosAggregatedVolume.Exporters;
using PetroineosAggregatedVolume.Interfaces;

namespace PetroineosAggregatedVolume.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddHostedService<Worker>();
            services.AddScoped<IExporter, CsvPositionsExporter>();
            services.AddScoped<IFileWriter, FileWriter>();
            services.AddScoped<IVolumeCalculator, VolumeCalculator>();
            return services;
        }
    }
}