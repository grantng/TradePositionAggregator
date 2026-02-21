using PetroineosAggregatedVolume.Models;

namespace PetroineosAggregatedVolume.Interfaces
{
    public interface IExporter
    {
        string ExportPositions(DateTime date, AggregatedPosition positions);
    }
}