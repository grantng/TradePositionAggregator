namespace TradePositionAggregator.Interfaces
{
    public interface IFileWriter
    {
        Task WriteToFileAsync(DateTime time, string contents);
    }
}