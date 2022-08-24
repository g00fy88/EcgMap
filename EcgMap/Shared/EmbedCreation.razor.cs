using FisSst.BlazorMaps;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Web;
using Yaapii.Atoms.Enumerable;
using Yaapii.Atoms.List;
using Yaapii.Atoms.Text;
using Yaapii.JSON;

namespace EcgMap.Shared
{
    public partial class EmbedCreation : ComponentBase
    {
        [Inject]
        public IClipboardService Clipboard { get; set; }
        [Inject]
        public ISnackbar Snackbar { get; set; }

        [Parameter]
        public string UrlBase { get; set; } = "";

        public string URLText { get; set; } = "";
        public string IFrameCode { get; set; } = ""; //"<iframe width=\"100%\" height=\"650\" src=\"\"></iframe>";
        public string FrameSrc { get; set; } = "";

        public void Create()
        {
            if(URLText != "")
            {
                this.FrameSrc = $"{UrlBase}?locationsUrl={HttpUtility.UrlEncode(URLText)}";
                this.IFrameCode = $"<iframe width=\"100%\" height=\"650\" frameborder=\"0\" scrolling=\"no\" marginheight=\"0\" marginwidth=\"0\" src=\"{FrameSrc}\" ></iframe>";
                StateHasChanged();
            }
        }

        public async Task CopyCode()
        {
            await this.Clipboard.CopyToClipboard(this.IFrameCode);
            this.Snackbar.Add("IFrame code was copied to clipboard", Severity.Success);
        }
    }
}
