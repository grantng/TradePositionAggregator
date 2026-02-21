using Microsoft.Extensions.Logging.Abstractions;
using PetroineosAggregatedVolume;
using PetroineosAggregatedVolume.Configuration;

namespace UnitTests
{
    public class FileWriterTests
    {
        private FileWriter _fileWriter;

        [SetUp]
        public void Setup()
        {
            _fileWriter = new FileWriter(NullLogger<FileWriter>.Instance);
        }

        [Test]
        public void WriteToFile_CreatesFileWithContents()
        {
            var now = DateTime.Now;
            var contents = "unit test contents";


            _fileWriter.WriteToFile(now, contents);

            var outputDirectory = AppSettings.GetAppSettings().OutputDirectory;
            var filename = $"PowerPosition_{now.ToString("yyyyMMdd_HHmm")}.csv";
            var path = Path.Join(outputDirectory, filename);

            try
            {
                Assert.That(File.Exists(path), Is.True, $"Expected file to exist at {path}");
                var read = File.ReadAllText(path);
                Assert.That(read, Is.EqualTo(contents));
            }
            finally
            {
                // cleanup
                if (File.Exists(path))
                    File.Delete(path);
            }
        }
    }
}