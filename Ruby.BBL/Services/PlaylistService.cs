using BBL.Providers;
using BBL.Services;
using Microsoft.EntityFrameworkCore;
using Ruby.DAL;
using Ruby.DAL.Entities;
using Ruby.Shared.DTO.PlaylistDTO;
using Ruby.Shared.DTO.TrackDTO;
using Ruby.Shared.DTO.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.BBL.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly RubyDBContext _context;
        private readonly IFileUrlProvider _urlProvider;
        private readonly IS3StorageService _s3StorageService;

        public PlaylistService(RubyDBContext context, IFileUrlProvider urlProvider, IS3StorageService s3StorageService)
        {
            _context = context;
            _urlProvider = urlProvider;
            _s3StorageService = s3StorageService;
        }

        public async Task<PlaylistResponseDTO> CreatePlaylistAsync(Guid currentUserId, CreatePlaylistRequestDTO dto)
        {
            var playlist = new PlaylistEntity
            {
                CreatorId = currentUserId,
                Title = dto.Title,
                CoverImageKey = dto.CoverImageKey,
                IsPrivate = dto.IsPrivate,
            };

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            return await GetPlaylistByIdAsync(playlist.Id, currentUserId);
        }

        public async Task<PlaylistResponseDTO> GetPlaylistByIdAsync(Guid playlistId, Guid? currentUserId = null)
        {
            var playlist = await _context.Playlists
                .AsNoTracking()
                .Include(p => p.Creator)
                .Include(p => p.PlaylistTracks)
                    .ThenInclude(pt => pt.Track)
                        .ThenInclude(t => t.Author)
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == playlistId);

            if (playlist == null) throw new Exception("Playlist not found");


            if (playlist.IsPrivate && playlist.CreatorId != currentUserId)
            {
                throw new Exception("This playlist is private");
            }

            return new PlaylistResponseDTO
            {
                Id = playlist.Id,
                Title = playlist.Title,
                CoverImageUrl = _urlProvider.GetPublicUrl(playlist.CoverImageKey),
                IsPrivate = playlist.IsPrivate,
                Author = new PublicUserShortResponseDTO
                {
                    Id = playlist.Creator.Id,
                    Username = playlist.Creator.Username
                },
                IsLikedByCurrentUser = currentUserId.HasValue && playlist.Likes.Any(l => l.UserId == currentUserId),
                Tracks = playlist.PlaylistTracks.Select(pt => new TrackShortResponseDTO
                {
                    Id = pt.Track.Id,
                    Title = pt.Track.Title,
                    DurationSeconds = pt.Track.DurationSeconds,
                    AudioFileUrl = _urlProvider.GetPublicUrl(pt.Track.AudioFileKey),
                    CoverImageUrl = _urlProvider.GetPublicUrl(pt.Track.CoverImageKey),
                    Author = new PublicUserShortResponseDTO
                    {
                        Id = pt.Track.Author.Id,
                        Username = pt.Track.Author.Username
                    }
                }).ToList()
            };
        }

        public async Task<List<PlaylistShortDTO>> GetLatestPlaylistsAsync(int count, Guid? userId)
        {
            var playlists = await _context.Playlists
                .AsNoTracking()
                .Include(p => p.PlaylistTracks)
                //берем только публичные плейлисты!
                .Where(p => !p.IsPrivate)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();

            return playlists.Select(p => new PlaylistShortDTO
            {
                Id = p.Id,
                Title = p.Title,
                CoverImageUrl = p.CoverImageKey != null ? _urlProvider.GetPublicUrl(p.CoverImageKey) : null,
                TracksCount = p.PlaylistTracks != null ? p.PlaylistTracks.Count : 0,
                IsLikedByCurrentUser = userId.HasValue &&
                               _context.PlaylistLikes.Any(l => l.PlaylistId == p.Id && l.UserId == userId.Value)
            }).ToList();
        }

        public async Task<List<PlaylistShortDTO>> GetPlaylistsByIdsAsync(List<Guid> ids, Guid? userId)
        {
            if (ids == null || !ids.Any()) return new List<PlaylistShortDTO>();


            // 1. Достаем из базы нужные плейлисты
            var playlists = await _context.Playlists
                .AsNoTracking()
                .Include(p => p.PlaylistTracks)
                .Where(p => ids.Contains(p.Id) && !p.IsPrivate)
                .ToListAsync();

            // 2. Маппим их в DTO
            var dtos = playlists.Select(p => new PlaylistShortDTO
            {
                Id = p.Id,
                Title = p.Title,
                CoverImageUrl = p.CoverImageKey != null ? _urlProvider.GetPublicUrl(p.CoverImageKey) : null,
                TracksCount = p.PlaylistTracks != null ? p.PlaylistTracks.Count : 0,
                IsLikedByCurrentUser = userId.HasValue &&
                               _context.PlaylistLikes.Any(l => l.PlaylistId == p.Id && l.UserId == userId.Value)
            }).ToList();

            // 3. Возвращаем в том же порядке, в котором передали ID
            return ids
                .Select(id => dtos.FirstOrDefault(d => d.Id == id))
                .Where(d => d != null) // Отсекаем null, если какой-то ID из конфига не найден в БД
                .ToList()!;
        }

        public async Task<PlaylistResponseDTO> UpdatePlaylistAsync(Guid currentUserId, Guid playlistId, UpdatePlaylistRequestDTO dto)
        {
            var playlist = await _context.Playlists.FirstOrDefaultAsync(p => p.Id == playlistId);

            if (playlist == null) throw new Exception("Playlist not found");
            if (playlist.CreatorId != currentUserId) throw new Exception("Access denied");

            if (!string.IsNullOrEmpty(dto.Title)) playlist.Title = dto.Title;
            if (dto.IsPrivate.HasValue) playlist.IsPrivate = dto.IsPrivate.Value;

            // Работа с BLOB (MinIO)
            if (dto.CoverImageKey != null && dto.CoverImageKey != playlist.CoverImageKey)
            {
                if (!string.IsNullOrEmpty(playlist.CoverImageKey))
                {
                    await _s3StorageService.DeleteFileAsync(playlist.CoverImageKey);
                }
                playlist.CoverImageKey = dto.CoverImageKey;
            }

            await _context.SaveChangesAsync();
            return await GetPlaylistByIdAsync(playlist.Id, currentUserId);
        }

        public async Task DeletePlaylistAsync(Guid currentUserId, Guid playlistId)
        {
            var playlist = await _context.Playlists.FirstOrDefaultAsync(p => p.Id == playlistId);

            if (playlist == null) throw new Exception("Playlist not found");
            if (playlist.CreatorId != currentUserId) throw new Exception("Access denied");

            // Удаляем обложку из MinIO
            if (!string.IsNullOrEmpty(playlist.CoverImageKey))
            {
                await _s3StorageService.DeleteFileAsync(playlist.CoverImageKey);
            }

            // Удаляем сам плейлист (треки внутри останутся живы, каскадно удалятся только связи)
            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();
        }

        // --- ДОБАВЛЕНИЕ И УДАЛЕНИЕ ТРЕКОВ ---

        public async Task AddTrackToPlaylistAsync(Guid currentUserId, Guid playlistId, Guid trackId)
        {
            var playlist = await _context.Playlists
         .Include(p => p.PlaylistTracks) // Мы уже подтягиваем все треки этого плейлиста!
         .FirstOrDefaultAsync(p => p.Id == playlistId);

            if (playlist == null) throw new Exception("Playlist not found");
            if (playlist.CreatorId != currentUserId) throw new Exception("Access denied");

            // ВАЖНАЯ ПРОВЕРКА: Если трек УЖЕ есть в плейлисте, просто молча выходим!
            // Никаких ошибок не бросаем, чтобы массовое добавление с фронтенда не прерывалось.
            if (playlist.PlaylistTracks.Any(pt => pt.TrackId == trackId))
            {
                return;
            }

            var trackExists = await _context.Tracks.AnyAsync(t => t.Id == trackId);
            if (!trackExists) throw new Exception("Track not found");

            var playlistTrack = new PlaylistTrackEntity
            {
                PlaylistId = playlistId,
                TrackId = trackId
            };

            _context.PlaylistTracks.Add(playlistTrack);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveTrackFromPlaylistAsync(Guid currentUserId, Guid playlistId, Guid trackId)
        {
            var playlistTrack = await _context.PlaylistTracks
                .Include(pt => pt.Playlist)
                .FirstOrDefaultAsync(pt => pt.PlaylistId == playlistId && pt.TrackId == trackId);

            if (playlistTrack == null) throw new Exception("Track is not in the playlist");
            if (playlistTrack.Playlist.CreatorId != currentUserId) throw new Exception("Access denied");

            _context.PlaylistTracks.Remove(playlistTrack);
            await _context.SaveChangesAsync();
        }

        // --- ЛАЙКИ И БИБЛИОТЕКА (Из предыдущих обсуждений) ---
        // Реализация GetLikedPlaylistsAsync и ToggleLikeAsync в точности такая же, 
        // как мы обсуждали ранее, я просто упоминаю их здесь для целостности картины.

        public async Task<List<PlaylistShortDTO>> GetLikedPlaylistsAsync(Guid currentUserId)
        {
            // Достаем все лайки юзера вместе с самими плейлистами
            var likedPlaylists = await _context.PlaylistLikes
                .AsNoTracking()
                .Include(pl => pl.Playlist)
                    .ThenInclude(p => p.PlaylistTracks) // Чтобы посчитать треки
                .Where(pl => pl.UserId == currentUserId)
                .OrderByDescending(pl => pl.CreatedAt) // Свежие лайки сверху
                .Select(pl => pl.Playlist) // Вытаскиваем сами плейлисты
                .ToListAsync();

            // Маппим в уже существующую DTO
            return likedPlaylists.Select(p => new PlaylistShortDTO
            {
                Id = p.Id,
                Title = p.Title,
                CoverImageUrl = p.CoverImageKey != null ? _urlProvider.GetPublicUrl(p.CoverImageKey) : null,
                TracksCount = p.PlaylistTracks != null ? p.PlaylistTracks.Count : 0
            }).ToList();
        }

        public async Task ToggleLikeAsync(Guid playlistId, Guid currentUserId)
        {
            // Проверяем, существует ли плейлист
            var playlistExists = await _context.Playlists.AnyAsync(p => p.Id == playlistId);
            if (!playlistExists) throw new Exception("Playlist not found");

            // Ищем, стоит ли уже лайк
            var existingLike = await _context.PlaylistLikes
                .FirstOrDefaultAsync(pl => pl.UserId == currentUserId && pl.PlaylistId == playlistId);

            if (existingLike != null)
            {
                // Лайк уже стоит -> удаляем (Снимаем лайк)
                _context.PlaylistLikes.Remove(existingLike);
            }
            else
            {
                // Лайка нет -> ставим
                _context.PlaylistLikes.Add(new PlaylistLikeEntity
                {
                    UserId = currentUserId,
                    PlaylistId = playlistId
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<PlaylistShortDTO>> GetMyLibraryAsync(Guid currentUserId)
        {
            // 1. Достаем плейлисты, которые юзер создал САМ
            var myCreatedPlaylists = await _context.Playlists
                .AsNoTracking()
                .Include(p => p.PlaylistTracks)
                .Where(p => p.CreatorId == currentUserId)
                .Select(p => new
                {
                    Playlist = p,
                    ActionDate = p.CreatedAt, // Для сортировки используем дату создания
                    IsLiked = _context.PlaylistLikes.Any(pl => pl.UserId == currentUserId && pl.PlaylistId == p.Id)
                })
                .ToListAsync();

            // 2. Достаем чужие плейлисты, которые юзер ЛАЙКНУЛ
            var myLikedPlaylists = await _context.PlaylistLikes
                .AsNoTracking()
                .Include(pl => pl.Playlist)
                    .ThenInclude(p => p.PlaylistTracks)
                .Where(pl => pl.UserId == currentUserId)
                .Select(pl => new
                {
                    Playlist = pl.Playlist,
                    ActionDate = pl.CreatedAt, // Для сортировки используем дату постановки лайка!
                    IsLiked = true
                })
                .ToListAsync();

            // 3. Склеиваем оба списка, сортируем так, чтобы свежие действия были сверху, и маппим в DTO
            var library = myCreatedPlaylists.Concat(myLikedPlaylists)
                .DistinctBy(x => x.Playlist.Id)
         .OrderByDescending(x => x.ActionDate)
         .Select(x => new PlaylistShortDTO
         {
             Id = x.Playlist.Id,
             Title = x.Playlist.Title,
             CoverImageUrl = x.Playlist.CoverImageKey != null ? _urlProvider.GetPublicUrl(x.Playlist.CoverImageKey) : null,
             TracksCount = x.Playlist.PlaylistTracks != null ? x.Playlist.PlaylistTracks.Count : 0,

             // ВОТ ОНО! Теперь фронтенд будет знать, закрашивать сердечко или нет
             IsLikedByCurrentUser = x.IsLiked
         })
         .ToList(); ;

            return library;
        }

        public async Task<List<PlaylistShortDTO>> GetMyCreatedPlaylistsAsync(Guid currentUserId)
        {
            var playlists = await _context.Playlists
                .AsNoTracking()
                .Include(p => p.PlaylistTracks)
                .Where(p => p.CreatorId == currentUserId) // Берем только те, где ты автор
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return playlists.Select(p => new PlaylistShortDTO
            {
                Id = p.Id,
                Title = p.Title,
                CoverImageUrl = p.CoverImageKey != null ? _urlProvider.GetPublicUrl(p.CoverImageKey) : null,
                TracksCount = p.PlaylistTracks != null ? p.PlaylistTracks.Count : 0
            }).ToList();
        }

        public async Task<List<PlaylistShortDTO>> SearchPlaylistsAsync(string query, Guid? currentUserId = null)
        {
            var lowerQuery = query.ToLower();

            // Ищем совпадения в названии плейлиста. Исключаем приватные плейлисты!
            var playlists = await _context.Playlists
                .Include(p => p.PlaylistTracks)
                .Where(p => !p.IsPrivate && p.Title.ToLower().Contains(lowerQuery))
                .Take(20)
                .ToListAsync();

            return playlists.Select(p => new PlaylistShortDTO
            {
                Id = p.Id,
                Title = p.Title,
                CoverImageUrl = p.CoverImageKey != null ? _urlProvider.GetPublicUrl(p.CoverImageKey) : null,
                TracksCount = p.PlaylistTracks?.Count ?? 0,
                IsLikedByCurrentUser = currentUserId.HasValue && _context.PlaylistLikes.Any(l => l.PlaylistId == p.Id && l.UserId == currentUserId.Value)
            }).ToList();
        }
    }
}