namespace EcgMap.Shared
{
    public interface IClipboardService
    {
        Task CopyToClipboard(string text);
    }
}
