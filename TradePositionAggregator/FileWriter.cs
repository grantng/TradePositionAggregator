using Microsoft.Extensions.Logging;
using PetroineosAggregatedVolume.Configuration;
using PetroineosAggregatedVolume.Interfaces;

namespace PetroineosAggregatedVolume
{
    public class FileWriter : IFileWriter
    {
        private ILogger<FileWriter> _logger;

        public FileWriter(ILogger<FileWriter> logger)
        {
            _logger = logger;
        }

        public void WriteToFile(DateTime time, string contents)
        {
            try
            {
                Directory.CreateDirectory(AppSettings.GetAppSettings().OutputDirectory);
                var path = GetPath(time);

                _logger.LogInformation($"Writing file: {path}");

                File.WriteAllText(path, contents);
            }
            catch (Exception e)
            {
                _logger.LogError($"Unable to write to file for: {time.ToString("yyyy-MM-dd HH:mm")}\nDetails: {e.Message}");
            }
        }


        private string GetFilename(DateTime time)
        {
            return $"PowerPosition_{time.ToString("yyyyMMdd_HHmm")}.csv";
        }

        private string GetPath(DateTime time)
        {
            var filename = GetFilename(time);
            var folder = AppSettings.GetAppSettings().OutputDirectory;
            var path = Path.Join(folder, filename);

            return path;
        }
    }
}
