using Microsoft.JSInterop;
using Ruby.UI.Services;
using System;
using System.Threading.Tasks;

namespace Ruby.Web.Services
{
    public class WebAudioEngine : IAudioEngine
    {
        private readonly IJSRuntime _js;
        private readonly DotNetObjectReference<WebAudioEngine> _objRef;
        private bool _isInitialized = false;

        public event Action<double, double>? OnProgress;
        public event Action? OnTrackEnded;

        public WebAudioEngine(IJSRuntime js)
        {
            _js = js;
            _objRef = DotNetObjectReference.Create(this);
        }

        private async Task EnsureInitializedAsync()
        {
            if (!_isInitialized)
            {
                await _js.InvokeVoidAsync("rubyAudioEngine.init", _objRef);
                _isInitialized = true;
            }
        }

        public async Task PlayAsync(string url)
        {
            await EnsureInitializedAsync();
            await _js.InvokeVoidAsync("rubyAudioEngine.play", url);
        }

        public async Task PauseAsync() => await _js.InvokeVoidAsync("rubyAudioEngine.pause");

        public async Task ResumeAsync() => await _js.InvokeVoidAsync("rubyAudioEngine.resume");

        public async Task StopAsync() => await _js.InvokeVoidAsync("rubyAudioEngine.stop");

        public async Task SeekAsync(double seconds) => await _js.InvokeVoidAsync("rubyAudioEngine.seek", seconds);

        public async Task SetVolumeAsync(int volume) => await _js.InvokeVoidAsync("rubyAudioEngine.setVolume", volume);


        [JSInvokable]
        public void OnJsProgress(double current, double total)
        {
            OnProgress?.Invoke(current, total);
        }

        [JSInvokable]
        public void OnJsTrackEnded()
        {
            OnTrackEnded?.Invoke();
        }

        public void Dispose()
        {
            _objRef?.Dispose();
        }
    }
}