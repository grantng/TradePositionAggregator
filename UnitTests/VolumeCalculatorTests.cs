using Microsoft.Extensions.Logging.Abstractions;
using TradePositionAggregator;
using Services;

namespace UnitTests
{
    public class VolumeCalculatorTests
    {
        VolumeCalculator _volumeCalculator;

        [SetUp]
        public void Setup()
        {
            _volumeCalculator = new VolumeCalculator(NullLogger<VolumeCalculator>.Instance);
        }

        [Test]
        public void AggregateTrades_SingleTrade_ReturnsSameVolumes()
        {
            var trade = PowerTradeFactory.CreatePowerTrade(DateTime.Now, new double[] { 1, 2, 3, 4, 5 });

            var trades = new List<PowerTrade> { trade };

            var aggregatedVolumes = _volumeCalculator.AggregateTrades(trades);

            Assert.That(aggregatedVolumes.Periods.Count, Is.EqualTo(trade.Periods.Length));

            foreach (var period in trade.Periods)
            {
                var aggregatedPeriod = aggregatedVolumes.Periods.FirstOrDefault(x => x.Period == period.Period);
                Assert.That(aggregatedPeriod, Is.Not.Null);
                Assert.That(aggregatedPeriod.Volume, Is.EqualTo(period.Volume));
            }
        }

        [Test]
        public void AggregateTrades_MultipleTrades_SumsVolumesPerPeriod()
        {
            var t1 = PowerTradeFactory.CreatePowerTrade(DateTime.Today, new double[] { 1, 2, 3 });
            var t2 = PowerTradeFactory.CreatePowerTrade(DateTime.Today, new double[] { 10, 20, 30 });
            var trades = new List<PowerTrade> { t1, t2 };

            var aggregated = _volumeCalculator.AggregateTrades(trades).Periods;

            Assert.That(aggregated.Count, Is.EqualTo(3));
            Assert.That(aggregated.First(x => x.Period == 1).Volume, Is.EqualTo(11));
            Assert.That(aggregated.First(x => x.Period == 2).Volume, Is.EqualTo(22));
            Assert.That(aggregated.First(x => x.Period == 3).Volume, Is.EqualTo(33));
        }

        [Test]
        public void AggregateTrades_EmptyInput_ReturnsEmptyList()
        {
            var aggregated = _volumeCalculator.AggregateTrades(new List<PowerTrade>());

            Assert.That(aggregated, Is.Not.Null);
            Assert.That(aggregated.Periods.Count, Is.EqualTo(0));
        }

        [Test]
        public void AggregateTrades_MultipleTrades_DifferentLengthPeriods()
        {
            var t1 = PowerTradeFactory.CreatePowerTrade(DateTime.Today, new double[] { 1, 2, 3 });
            var t2 = PowerTradeFactory.CreatePowerTrade(DateTime.Today, new double[] { 10, 20, 30, 40, 50 });
            var trades = new List<PowerTrade> { t1, t2 };

            var aggregated = _volumeCalculator.AggregateTrades(trades).Periods;

            Assert.That(aggregated.Count, Is.EqualTo(5));
            Assert.That(aggregated.First(x => x.Period == 1).Volume, Is.EqualTo(11));
            Assert.That(aggregated.First(x => x.Period == 2).Volume, Is.EqualTo(22));
            Assert.That(aggregated.First(x => x.Period == 3).Volume, Is.EqualTo(33));
            Assert.That(aggregated.First(x => x.Period == 4).Volume, Is.EqualTo(40));
            Assert.That(aggregated.First(x => x.Period == 5).Volume, Is.EqualTo(50));
        }
    }
}
