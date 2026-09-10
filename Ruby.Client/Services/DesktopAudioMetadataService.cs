using ManagedBass;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using Ruby.UI.Services;

namespace Ruby.Client.Services
{
    public class DesktopAudioMetadataService : IAudioMetadataService
    {
        public async Task<int> GetTrackDurationAsync(IBrowserFile file)
        {
            string tempPath = Path.GetTempFileName();
            try
            {
                using (var fs = new FileStream(tempPath, FileMode.Create))
                {
                    await file.OpenReadStream(maxAllowedSize: 30 * 1024 * 1024).CopyToAsync(fs);
                }

                Bass.Init(-1, 44100, DeviceInitFlags.Default, IntPtr.Zero);
                int stream = Bass.CreateStream(tempPath, 0, 0, BassFlags.Decode | BassFlags.Prescan);

                if (stream != 0)
                {
                    long lengthBytes = Bass.ChannelGetLength(stream);
                    double duration = Bass.ChannelBytes2Seconds(stream, lengthBytes);
                    Bass.StreamFree(stream);
                    return (int)Math.Round(duration);
                }

                return 0;
            }
            finally
            {
                if (File.Exists(tempPath)) File.Delete(tempPath);
            }
        }
    }
}