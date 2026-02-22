using Services;

namespace TradePositionAggregator.Interfaces
{
    public interface ITradeRepository
    {
        IEnumerable<PowerTrade> GetTrades(DateTime date);
        Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date);
    }
}