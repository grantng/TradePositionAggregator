using TradePositionAggregator.Models;

namespace TradePositionAggregator.Interfaces
{
    public interface IExporter
    {
        string ExportPositions(DateTime date, AggregatedPosition positions);
    }
}