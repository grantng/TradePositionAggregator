using Services;
using TradePositionAggregator.Interfaces;

namespace TradePositionAggregator
{
    public class TradeRepository : ITradeRepository
    {
        PowerService _powerService;

        public TradeRepository()
        {
            _powerService = new PowerService();
        }

        public IEnumerable<PowerTrade> GetTrades(DateTime date)
        {
            return _powerService.GetTrades(date);
        }

        public Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date)
        {
            return _powerService.GetTradesAsync(date);
        }
    }
}
