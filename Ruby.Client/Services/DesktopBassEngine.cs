using ManagedBass;
using Ruby.UI.Services;
using System.Timers;

namespace Ruby.Client.Services
{
    public class DesktopBassEngine : IAudioEngine
    {
        private int _stream;
        private readonly System.Timers.Timer _playbackTimer;
        private SyncProcedure _endSync;

        public event Action<double, double>? OnProgress;
        public event Action? OnTrackEnded;

        public DesktopBassEngine()
        {
            Bass.Init(-1, 44100, DeviceInitFlags.Default, IntPtr.Zero);

            _playbackTimer = new System.Timers.Timer(500);
            _playbackTimer.Elapsed += Timer_Elapsed;

            _endSync = new SyncProcedure((handle, channel, data, user) =>
            {
                Task.Run(() => OnTrackEnded?.Invoke());
            });
        }

        public Task PlayAsync(string url)
        {
            if (_stream != 0) Bass.StreamFree(_stream);

            _stream = Bass.CreateStream(url, 0, BassFlags.Default, null, IntPtr.Zero);

            if (_stream != 0)
            {
                Bass.ChannelSetSync(_stream, SyncFlags.End, 0, _endSync);
                Bass.ChannelPlay(_stream);
                _playbackTimer.Start();
            }
            return Task.CompletedTask;
        }

        public Task PauseAsync() { Bass.ChannelPause(_stream); _playbackTimer.Stop(); return Task.CompletedTask; }

        public Task ResumeAsync() { Bass.ChannelPlay(_stream); _playbackTimer.Start(); return Task.CompletedTask; }

        public Task StopAsync() { Bass.ChannelStop(_stream); _playbackTimer.Stop(); return Task.CompletedTask; }

        public Task SeekAsync(double seconds)
        {
            long positionBytes = Bass.ChannelSeconds2Bytes(_stream, seconds);
            Bass.ChannelSetPosition(_stream, positionBytes);
            return Task.CompletedTask;
        }

        public Task SetVolumeAsync(int volume)
        {
            Bass.GlobalStreamVolume = volume * 100;
            return Task.CompletedTask;
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (_stream != 0 && Bass.ChannelIsActive(_stream) == PlaybackState.Playing)
            {
                long posBytes = Bass.ChannelGetPosition(_stream);
                long lenBytes = Bass.ChannelGetLength(_stream);

                double current = Bass.ChannelBytes2Seconds(_stream, posBytes);
                double total = Bass.ChannelBytes2Seconds(_stream, lenBytes);

                OnProgress?.Invoke(current, total);
            }
        }

        public void Dispose()
        {
            _playbackTimer.Dispose();
            if (_stream != 0) Bass.StreamFree(_stream);
            Bass.Free();
        }
    }
}