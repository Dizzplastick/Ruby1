using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Ruby.UI.Services;

namespace Ruby.Web.Services
{
    public class WebAudioMetadataService : IAudioMetadataService
    {
        private readonly IJSRuntime _js;

        public WebAudioMetadataService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<int> GetTrackDurationAsync(IBrowserFile file)
        {
            using var stream = file.OpenReadStream(maxAllowedSize: 30 * 1024 * 1024);
            using var streamRef = new DotNetStreamReference(stream);

            return await _js.InvokeAsync<int>("rubyAudioHelpers.getAudioDuration", streamRef);
        }
    }
}