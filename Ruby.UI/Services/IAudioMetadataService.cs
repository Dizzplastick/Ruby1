using Microsoft.AspNetCore.Components.Forms;

namespace Ruby.UI.Services
{
    public interface IAudioMetadataService
    {
        Task<int> GetTrackDurationAsync(IBrowserFile file);
    }
}