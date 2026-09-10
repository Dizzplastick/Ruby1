using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.UI.Services
{
    public interface IAudioEngine : IDisposable
    {
        Task PlayAsync(string url);
        Task PauseAsync();
        Task ResumeAsync();
        Task StopAsync();
        Task SeekAsync(double seconds);
        Task SetVolumeAsync(int volume); 

        event Action<double, double>? OnProgress; 
        event Action? OnTrackEnded;               
    }
}