using BBL.Providers;
using BBL.Services;
using Microsoft.EntityFrameworkCore;
using Ruby.DAL;
using Ruby.DAL.Entities;
using Ruby.Shared.DTO.LikesDTO;
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
    public class TrackService : ITrackService
    {
        private readonly RubyDBContext _context;
        private readonly IFileUrlProvider _urlProvider;
        private readonly IS3StorageService _s3StorageService;

        public TrackService (RubyDBContext context, IFileUrlProvider urlProvider, IS3StorageService s3StorageService)
        {
            _context = context;
            _urlProvider = urlProvider;
            _s3StorageService = s3StorageService;
        }


        public async Task<TrackResponseDTO> CreateTrackAsync(Guid curentUserId, CreateTrackRequestDTO dto)
        {
            var track = new TrackEntity
            {
                Title = dto.Title,
                AuthorId = curentUserId,
                AudioFileKey = dto.AudioFileKey,
                CoverImageKey = dto.CoverImageKey,
                DurationSeconds = dto.DurationSeconds,
            };

            _context.Tracks.Add(track);
            await _context.SaveChangesAsync();

            if (dto.GenreIds != null && dto.GenreIds.Any())
            {
                var trackGenres = dto.GenreIds.Select(genreId => new TrackGenreEntity
                {
                    TrackId = track.Id,
                    GenreId = genreId
                });

                _context.TrackGenres.AddRange(trackGenres);
                await _context.SaveChangesAsync(); 
            }

            return await GetTrackByIdAsync(track.Id , curentUserId);
        }

        public async Task DeleteTrackAsync(Guid currentUserId, Guid trackId)
        {
            var track = await _context.Tracks.FirstOrDefaultAsync(t => t.Id == trackId);

            if (track == null) throw new Exception("Track not found");
            if (track.AuthorId != currentUserId) throw new Exception("You dont have permission to delete this track");

            await _s3StorageService.DeleteFileAsync(track.AudioFileKey);
            if (!string.IsNullOrEmpty(track.CoverImageKey))
            {
                await _s3StorageService.DeleteFileAsync(track.CoverImageKey);
            }

            _context.Tracks.Remove(track);
            await _context.SaveChangesAsync();

        }

        public async Task<List<TrackShortResponseDTO>> GetLatestTracksAsync(int count = 10, Guid? currentUserId = null)
        {
            var tracks = await _context.Tracks
                .AsNoTracking() 
                .Include(t => t.Author) 
                .OrderByDescending(t => t.CreatedAt) 
                .Take(count)
                .ToListAsync();

            return tracks.Select(t => new TrackShortResponseDTO
            {
                Id = t.Id,
                Title = t.Title,
                DurationSeconds = t.DurationSeconds,
                AudioFileUrl = _urlProvider.GetPublicUrl(t.AudioFileKey),
                CoverImageUrl = _urlProvider.GetPublicUrl(t.CoverImageKey),
                Author = new PublicUserShortResponseDTO
                {
                    Id = t.Author.Id,
                    Username = t.Author.Username
                },
                IsLikedByCurrentUser = currentUserId.HasValue &&
                               _context.Likes.Any(l => l.TrackId == t.Id && l.UserId == currentUserId.Value)
            }).ToList();
        }

        public async Task<TrackResponseDTO> GetTrackByIdAsync(Guid trackId, Guid currentUserId)
        {
            var track = await _context.Tracks
                .AsNoTracking()
                .Include(t => t.Author)
                .FirstOrDefaultAsync(track => track.Id == trackId);


            if (track == null) throw new Exception("Track not found");

            return new TrackResponseDTO
            {
                Id = trackId,
                Title = track.Title,
                AudioFileUrl = _urlProvider.GetPublicUrl(track.AudioFileKey),
                CoverImageUrl = _urlProvider.GetPublicUrl(track.CoverImageKey),
                CreatedAt = track.CreatedAt,
                LikesCount = track.Likes != null ? track.Likes.Count : 0,
                IsLikedByCurrentUser = currentUserId != null && track.Likes != null && track.Likes.Any(l => l.UserId == currentUserId),
                Author = new PublicUserShortResponseDTO
                {
                    Id = track.Author.Id,
                    Username = track.Author.Username
                }
            };
        }

        public async Task<TrackResponseDTO> UpdateTrackAsync(Guid currentUserId, Guid trackId, UpdateTrackRequestDTO dto)
        {
            var track = await _context.Tracks
                .Include(t => t.TrackGenres)
                .FirstOrDefaultAsync(t => t.Id == trackId);


            if (track == null) throw new Exception("Track not found");

            if (track.AuthorId != currentUserId) throw new Exception("You dont hane permission to update this track");

            if (!string.IsNullOrEmpty(dto.Title))
            {
                track.Title = dto.Title;
            }

            if (dto.CoverImageKey !=null && dto.CoverImageKey != track.CoverImageKey)
            {
                if (!string.IsNullOrEmpty(track.CoverImageKey))
                {
                    await _s3StorageService.DeleteFileAsync(track.CoverImageKey);
                }

                track.CoverImageKey = dto.CoverImageKey;
            }

            if(dto.GenreIds != null)
            {
                if (track.TrackGenres != null && track.TrackGenres.Any())
                {
                    _context.TrackGenres.RemoveRange(track.TrackGenres);
                }

                if (dto.GenreIds.Any())
                {
                    var newGenres = dto.GenreIds.Select(genreId => new TrackGenreEntity
                    {
                        TrackId = track.Id,
                        GenreId = genreId
                    });

                    _context.TrackGenres.AddRange(newGenres);
                }
            }

            await _context.SaveChangesAsync();

            return await GetTrackByIdAsync(track.Id , currentUserId);
        }

        public async Task ToggleLikeAsync (Guid currentUserId, ToggleLikeRequestDTO dto)
        {
            var trackExists = await _context.Tracks.AnyAsync(t => t.Id == dto.TrackId);
            if (!trackExists) throw new Exception("Track not found");

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == currentUserId && l.TrackId == dto.TrackId);

            if (existingLike != null) 
            {
                _context.Likes.Remove(existingLike);

            }
            else
            {
                _context.Likes.Add(new LikeEntity
                {
                    UserId = currentUserId,
                    TrackId = dto.TrackId
                });
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<TrackShortResponseDTO>> SearchTracksAsync(string query, Guid? currentUserId = null)
        {
            var lowerQuery = query.ToLower();

            var tracks = await _context.Tracks
                .Include(t => t.Author)
                .Where(t => t.Title.ToLower().Contains(lowerQuery) ||
                            t.Author.Username.ToLower().Contains(lowerQuery))
                .Take(50) 
                .ToListAsync();

            return tracks.Select(t => new TrackShortResponseDTO
            {
                Id = t.Id,
                Title = t.Title,
                DurationSeconds = t.DurationSeconds,
                Author = new PublicUserShortResponseDTO { Id = t.Author.Id, Username = t.Author.Username },
                AudioFileUrl = _urlProvider.GetPublicUrl(t.AudioFileKey),
                CoverImageUrl = t.CoverImageKey != null ? _urlProvider.GetPublicUrl(t.CoverImageKey) : null,
                IsLikedByCurrentUser = currentUserId.HasValue && _context.Likes.Any(l => l.TrackId == t.Id && l.UserId == currentUserId.Value)
            }).ToList();
        }

        


        public async Task<List<TrackShortResponseDTO>> GetLikedTracksAsync(Guid currentUserId, int? count = null)
        {
            var query = _context.Likes
                .AsNoTracking()
                .Include(l => l.Track)
                    .ThenInclude(t => t.Author)
                .Where(l => l.UserId == currentUserId)
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => l.Track);

            if (count.HasValue && count.Value > 0)
            {
                query = query.Take(count.Value);
            }

            var likedTracks = await query.ToListAsync();

            return likedTracks.Select(t => new TrackShortResponseDTO
            {
                Id = t.Id,
                Title = t.Title,
                DurationSeconds = t.DurationSeconds,
                AudioFileUrl = _urlProvider.GetPublicUrl(t.AudioFileKey),
                CoverImageUrl = t.CoverImageKey != null ? _urlProvider.GetPublicUrl(t.CoverImageKey) : null,
                Author = new PublicUserShortResponseDTO
                {
                    Id = t.Author.Id,
                    Username = t.Author.Username
                },
                IsLikedByCurrentUser = true
            }).ToList();
        }
    }
}
