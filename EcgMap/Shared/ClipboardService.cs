using Microsoft.JSInterop;

namespace EcgMap.Shared
{
    public sealed class ClipboardService : IClipboardService
    {
        private readonly IJSRuntime jsInterop;

        public ClipboardService(IJSRuntime jsInterop)
        {
            this.jsInterop = jsInterop;
        }

        public async Task CopyToClipboard(string text)
        {
            await this.jsInterop.InvokeVoidAsync("navigator.clipboard.writeText", text);
        }
    }
}
