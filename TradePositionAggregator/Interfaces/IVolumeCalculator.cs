using TradePositionAggregator.Models;
using Services;

namespace TradePositionAggregator.Interfaces
{
    public interface IVolumeCalculator
    {
        AggregatedPosition AggregateTrades(IEnumerable<PowerTrade> trades);
    }
}