using Microsoft.Extensions.Logging.Abstractions;
using PetroineosAggregatedVolume;
using PetroineosAggregatedVolume.Exporters;
using Services;

namespace UnitTests
{
    public class CsvPositionsExporterTests
    {
        private CsvPositionsExporter _exporter;
        private VolumeCalculator _volumeCalculator;

        [SetUp]
        public void Setup()
        {
            _exporter = new CsvPositionsExporter(NullLogger<CsvPositionsExporter>.Instance);
            _volumeCalculator = new VolumeCalculator(NullLogger<VolumeCalculator>.Instance);
        }

        [Test]
        public void ExportPositions_IncludesHeaderAndAllPeriods()
        {
            // Create a small aggregated position
            var trade = PowerTradeFactory.CreatePowerTrade(DateTime.Now, new double[] { 10, 20, 30, 40, 50, 60 });

            var trades = new List<PowerTrade> { trade };

            var aggregate = _volumeCalculator.AggregateTrades(trades);


            var date = new DateTime(2026, 1, 1);
            var csv = _exporter.ExportPositions(date, aggregate);

            var lines = csv.TrimEnd().Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            Assert.That(lines.Length, Is.EqualTo(1 + aggregate.Periods.Count)); // header + periods
            Assert.That(lines[0], Is.EqualTo("Local Time,Volume"));

            // Ensure volumes appear in CSV
            var times = lines.Skip(1).Select(l => l.Split(',')[0]).ToArray();
            Assert.That(times, Is.EquivalentTo(new[] { "23:00", "00:00", "01:00", "02:00", "03:00", "04:00" }));

            // Ensure local times appear in CSV
            var volumes = lines.Skip(1).Select(l => l.Split(',')[1]).ToArray();
            Assert.That(volumes, Is.EquivalentTo(new[] { "10", "20", "30", "40", "50", "60" }));
        }

        [Test]
        public void ExportPositions_DaylightSavingShortDay()
        {
            // Create a small aggregated position
            var trade = PowerTradeFactory.CreatePowerTrade(DateTime.Now, new double[] { 10, 20, 30, 40, 50, 60 });

            var trades = new List<PowerTrade> { trade };

            var aggregate = _volumeCalculator.AggregateTrades(trades);


            var date = new DateTime(2026, 3, 29);
            var csv = _exporter.ExportPositions(date, aggregate);

            var lines = csv.TrimEnd().Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            Assert.That(lines.Length, Is.EqualTo(1 + aggregate.Periods.Count)); // header + periods
            Assert.That(lines[0], Is.EqualTo("Local Time,Volume"));

            // Ensure volumes appear in CSV
            var times = lines.Skip(1).Select(l => l.Split(',')[0]).ToArray();
            Assert.That(times, Is.EquivalentTo(new[] { "23:00", "00:00", "02:00", "03:00", "04:00", "05:00", }));

            // Ensure local times appear in CSV
            var volumes = lines.Skip(1).Select(l => l.Split(',')[1]).ToArray();
            Assert.That(volumes, Is.EquivalentTo(new[] { "10", "20", "30", "40", "50", "60" }));
        }

        [Test]
        public void ExportPositions_DaylightSavingLongDay()
        {
            // Create a small aggregated position
            var trade = PowerTradeFactory.CreatePowerTrade(DateTime.Now, new double[] { 10, 20, 30, 40, 50, 60 });

            var trades = new List<PowerTrade> { trade };

            var aggregate = _volumeCalculator.AggregateTrades(trades);


            var date = new DateTime(2026, 10, 25);
            var csv = _exporter.ExportPositions(date, aggregate);

            var lines = csv.TrimEnd().Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            Assert.That(lines.Length, Is.EqualTo(1 + aggregate.Periods.Count)); // header + periods
            Assert.That(lines[0], Is.EqualTo("Local Time,Volume"));

            // Ensure volumes appear in CSV
            var times = lines.Skip(1).Select(l => l.Split(',')[0]).ToArray();
            Assert.That(times, Is.EquivalentTo(new[] { "23:00", "00:00", "01:00", "01:00", "02:00", "03:00" }));

            // Ensure local times appear in CSV
            var volumes = lines.Skip(1).Select(l => l.Split(',')[1]).ToArray();
            Assert.That(volumes, Is.EquivalentTo(new[] { "10", "20", "30", "40", "50", "60" }));
        }
    }
}