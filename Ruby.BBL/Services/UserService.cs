using BBL.Providers;
using Microsoft.EntityFrameworkCore;
using Ruby.DAL;
using Ruby.Shared.DTO.PlaylistDTO;
using Ruby.Shared.DTO.TrackDTO;
using Ruby.Shared.DTO.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBL.Services
{
    public class UserService : IUserService
    {
        private readonly RubyDBContext _context;
        private readonly IFileUrlProvider _urlProvider;
        private readonly IS3StorageService _s3StorageService;

        public UserService(RubyDBContext context, IFileUrlProvider urlProvider, IS3StorageService s3StorageService)
        {
            _context = context;
            _urlProvider = urlProvider;
            _s3StorageService = s3StorageService;
        }



        public async Task<CurrentUserResponseDTO> GetCurrentUserProfileAsync(Guid currentUserId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            return new CurrentUserResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                AvatarUrl = user.AvatarKey != null ? _urlProvider.GetPublicUrl(user.AvatarKey) : null,
            };
        }

        public async Task<PublicUserResponseDTO> GetPublicUserProfileAsync(Guid userId, Guid? currentUserId = null)
        {
            var user = await _context.Users
                .Include(u => u.Tracks)
                .Include(u => u.Playlists)
                    .ThenInclude(p => p.PlaylistTracks)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var likedTrackIds = new HashSet<Guid>();
            var likedPlaylistIds = new HashSet<Guid>();

            if (currentUserId.HasValue)
            {
                var profileTrackIds = user.Tracks.Select(t => t.Id).ToList();
                var profilePlaylistIds = user.Playlists.Select(p => p.Id).ToList();

                var trackLikesList = await _context.Likes
            .Where(tl => tl.UserId == currentUserId.Value && profileTrackIds.Contains(tl.TrackId))
            .Select(tl => tl.TrackId)
            .ToListAsync(); 

                likedTrackIds = trackLikesList.ToHashSet(); 

                var playlistLikesList = await _context.PlaylistLikes
                    .Where(pl => pl.UserId == currentUserId.Value && profilePlaylistIds.Contains(pl.PlaylistId))
                    .Select(pl => pl.PlaylistId)
                    .ToListAsync(); // Выгружаем из БД асинхронно

                likedPlaylistIds = playlistLikesList.ToHashSet();
            }

            return new PublicUserResponseDTO
            {
                Id = user.Id,
                Username = user.Username,
                AvatarUrl = user.AvatarKey != null ? _urlProvider.GetPublicUrl(user.AvatarKey) : null,

                Tracks = user.Tracks.Select(t => new TrackShortResponseDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    DurationSeconds = t.DurationSeconds,
                    Author = new PublicUserShortResponseDTO
                    {
                        Id = user.Id,
                        Username = user.Username
                    },
                    AudioFileUrl = _urlProvider.GetPublicUrl(t.AudioFileKey),
                    CoverImageUrl = t.CoverImageKey != null ? _urlProvider.GetPublicUrl(t.CoverImageKey) : null,
                    IsLikedByCurrentUser = likedTrackIds.Contains(t.Id)
                }).ToList(),

                Playlists = user.Playlists.Select(p => new PlaylistShortDTO
                {
                    Id = p.Id,
                    Title = p.Title,
                    CoverImageUrl = p.CoverImageKey != null ? _urlProvider.GetPublicUrl(p.CoverImageKey) : null,

                    TracksCount = p.PlaylistTracks != null ? p.PlaylistTracks.Count : 0,
                    IsLikedByCurrentUser = likedPlaylistIds.Contains(p.Id)
                }).ToList()
            };
        }

        public async Task UpdatePasswordAsync(Guid currentUserId, UpdatePasswordRequestDTO dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            bool isOldPasswordValid = BCrypt.Net.BCrypt.Verify(dto.OldPassword, user.HashPassword);

            if (!isOldPasswordValid)
            {
                throw new Exception("The current password is incorrect");
            }

            string newHashPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            user.HashPassword = newHashPassword;

            var activeTokens = await _context.RefreshTokens
        .Where(rt => rt.UserId == currentUserId)
        .ToListAsync();

            if (activeTokens.Any())
            {
                _context.RefreshTokens.RemoveRange(activeTokens);
            }


            await _context.SaveChangesAsync();

        }

        public async Task<CurrentUserResponseDTO> UpdateProfileAsync(Guid currentUserId, UpdateProfileRequestDTO dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (!string.IsNullOrEmpty(dto.Username))
            {
                user.Username = dto.Username;
            }

            if (!string.IsNullOrEmpty(dto.Username) && dto.Username != user.Username)
            {
                bool isUsernameTaken = await _context.Users.AnyAsync(u => u.Username == dto.Username);
                if (isUsernameTaken) throw new Exception("This username is already taken");
                user.Username = dto.Username;
            }

            if (!string.IsNullOrEmpty(dto.Email))
            {
                user.Email = dto.Email;
            }

            if (!string.IsNullOrEmpty(dto.AvatarKey))
            {
                if (!string.IsNullOrEmpty(user.AvatarKey))
                {
                    await _s3StorageService.DeleteFileAsync(user.AvatarKey);
                }
                user.AvatarKey = dto.AvatarKey;
            }

            await _context.SaveChangesAsync();

            return new CurrentUserResponseDTO {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                AvatarUrl = user.AvatarKey != null ? _urlProvider.GetPublicUrl(user.AvatarKey) : null,
                CreatedAt = user.CreatedAt
            };

        }

    }
}
