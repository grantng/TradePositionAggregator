namespace PetroineosAggregatedVolume.Interfaces
{
    public interface IFileWriter
    {
        void WriteToFile(DateTime time, string contents);
    }
}