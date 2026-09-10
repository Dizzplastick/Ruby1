using Ruby.Shared.DTO.TrackDTO;

namespace Ruby.UI.Services
{
    public enum RepeatMode { None, All, One }

    public class AudioPlayerService : IDisposable
    {
        private readonly IAudioEngine _engine;

        public event Action? OnStateChanged;

        public List<TrackShortResponseDTO> CurrentQueue { get; private set; } = new();
        public int CurrentIndex { get; private set; } = -1;
        public TrackShortResponseDTO? CurrentTrack =>
            CurrentIndex >= 0 && CurrentIndex < CurrentQueue.Count ? CurrentQueue[CurrentIndex] : null;

        public bool IsPlaying { get; private set; }
        public bool IsShuffle { get; private set; }
        public RepeatMode Repeat { get; private set; } = RepeatMode.None;

        public double CurrentPositionSeconds { get; private set; }
        public double TotalDurationSeconds { get; private set; }
        public int CurrentVolume { get; private set; } = 80;

        public AudioPlayerService(IAudioEngine engine)
        {
            _engine = engine;

            _engine.OnProgress += (current, total) =>
            {
                CurrentPositionSeconds = current;
                TotalDurationSeconds = total;
                NotifyStateChanged();
            };

            _engine.OnTrackEnded += () =>
            {
                IsPlaying = false;
                Next(); 
            };
        }

        public async Task PlayPlaylistAsync(List<TrackShortResponseDTO> tracks, int startIndex = 0)
        {
            CurrentQueue = tracks.ToList();
            CurrentIndex = startIndex;
            await PlayCurrentAsync();
        }

        public async Task PlayTrackAsync(TrackShortResponseDTO track)
        {
            CurrentQueue = new List<TrackShortResponseDTO> { track };
            CurrentIndex = 0;
            await PlayCurrentAsync();
        }

        private async Task PlayCurrentAsync()
        {
            if (CurrentTrack == null) return;

            await _engine.PlayAsync(CurrentTrack.AudioFileUrl);
            await _engine.SetVolumeAsync(CurrentVolume);

            IsPlaying = true;
            NotifyStateChanged();
        }

        public async Task Next()
        {
            if (CurrentQueue.Count == 0) return;

            if (Repeat == RepeatMode.One)
            {
                await PlayCurrentAsync();
                return;
            }

            if (IsShuffle)
            {
                CurrentIndex = new Random().Next(0, CurrentQueue.Count);
            }
            else
            {
                CurrentIndex++;
                if (CurrentIndex >= CurrentQueue.Count)
                {
                    if (Repeat == RepeatMode.All) CurrentIndex = 0;
                    else { await _engine.StopAsync(); IsPlaying = false; NotifyStateChanged(); return; }
                }
            }
            await PlayCurrentAsync();
        }

        public async Task Previous()
        {
            if (IsPlaying && CurrentPositionSeconds > 3)
            {
                await SeekAsync(0);
                return;
            }

            if (CurrentQueue.Count == 0) return;

            CurrentIndex--;
            if (CurrentIndex < 0) CurrentIndex = Repeat == RepeatMode.All ? CurrentQueue.Count - 1 : 0;

            await PlayCurrentAsync();
        }

        public void ToggleShuffle() { IsShuffle = !IsShuffle; NotifyStateChanged(); }

        public void ToggleRepeat()
        {
            Repeat = Repeat switch
            {
                RepeatMode.None => RepeatMode.All,
                RepeatMode.All => RepeatMode.One,
                RepeatMode.One => RepeatMode.None,
                _ => RepeatMode.None
            };
            NotifyStateChanged();
        }

        public async Task TogglePlayPause()
        {
            if (CurrentTrack == null) return;

            if (IsPlaying) { await _engine.PauseAsync(); IsPlaying = false; }
            else { await _engine.ResumeAsync(); IsPlaying = true; }

            NotifyStateChanged();
        }

        public async Task SeekAsync(double seconds)
        {
            await _engine.SeekAsync(seconds);
            CurrentPositionSeconds = seconds;
            NotifyStateChanged();
        }

        public async Task SetVolumeAsync(int volume)
        {
            CurrentVolume = Math.Clamp(volume, 0, 100);
            await _engine.SetVolumeAsync(CurrentVolume);
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnStateChanged?.Invoke();

        public void Dispose() => _engine.Dispose();
    }
}