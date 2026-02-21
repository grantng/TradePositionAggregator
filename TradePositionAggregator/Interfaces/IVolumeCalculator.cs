using PetroineosAggregatedVolume.Models;
using Services;

namespace PetroineosAggregatedVolume.Interfaces
{
    public interface IVolumeCalculator
    {
        AggregatedPosition AggregateTrades(IEnumerable<PowerTrade> trades);
    }
}