using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BBL.Providers;
using Ruby.DAL;
using Ruby.DAL.Entities;
using Ruby.Shared.DTO.UserDTO;

namespace BBL.Services
{
    public class AuthService : IAuthService
    {
        private readonly RubyDBContext _context;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly IFileUrlProvider _urlProvider;
        IRefreshTokenProvider _refreshTokenProvider;

        public AuthService(RubyDBContext context, IJwtTokenProvider jwtTokenProvider, IFileUrlProvider urlProvider, IRefreshTokenProvider refreshTokenProvider)
        {
            _context = context;
            _jwtTokenProvider = jwtTokenProvider;
            _urlProvider = urlProvider;
            _refreshTokenProvider = refreshTokenProvider;
        }

        public async Task<AuthResponseDTO> RegisterAsync (UserRegisterRequestDTO dto)
        {

            bool userExists = await _context.Users
                .AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email);
            if (userExists)
            {
                
                throw new Exception("A user with that name or email address already exists..");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var newUser = new UserEntity
            {
                Username = dto.Username,
                Email = dto.Email,
                HashPassword = passwordHash,
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();


            string jwtToken = _jwtTokenProvider.GenerateJwtToken(newUser);
            string refreshToken = _refreshTokenProvider.GenerateRefreshToken();

            var newRefreshToken = new RefreshTokenEntity
            {
                UserId = newUser.Id,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();


            return new AuthResponseDTO
            {
                Token = jwtToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                RefreshToken = refreshToken,
                User = new CurrentUserResponseDTO
                {
                    Id = newUser.Id,
                    Username = newUser.Username,
                    Email = newUser.Email,
                    AvatarUrl = _urlProvider.GetPublicUrl(newUser.AvatarKey),
                }
            };
        }

        public async Task<AuthResponseDTO> LoginAsync(UserLogInRequestDTO dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null)
            {
                throw new Exception("Incorrect username or password.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.HashPassword);

            if (!isPasswordValid)
            {
                throw new Exception("Incorrect username or password.");
            }

            string jwtToken = _jwtTokenProvider.GenerateJwtToken(user);
            string refreshToken = _refreshTokenProvider.GenerateRefreshToken();

            var newRefreshToken = new RefreshTokenEntity
            {
                UserId = user.Id,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponseDTO
            {
                Token = jwtToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                RefreshToken = refreshToken,
                User = new CurrentUserResponseDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    AvatarUrl = user.AvatarKey != null ? _urlProvider.GetPublicUrl(user.AvatarKey) : null
                }
            };
        }

        public async Task<AuthResponseDTO> RefreshTokenAsync(RefreshJWTRequestDTO dto)
        {

            var oldTokenEntity = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.RefreshToken == dto.RefreshToken);

            if (oldTokenEntity == null)
            {
                throw new Exception("Invalid update token.");
            }

            if (oldTokenEntity.ExpiresAt < DateTime.UtcNow)
            {
                _context.RefreshTokens.Remove(oldTokenEntity);
                await _context.SaveChangesAsync();
                throw new Exception("Your session has expired. Please log in again.");
            }

            _context.RefreshTokens.Remove(oldTokenEntity);

            var user = oldTokenEntity.User;
            string newJwtToken = _jwtTokenProvider.GenerateJwtToken(user);
            string newRefreshToken = _refreshTokenProvider.GenerateRefreshToken();

            var newRefreshTokenEntity = new RefreshTokenEntity
            {
                UserId = user.Id,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
            };

            _context.RefreshTokens.Add(newRefreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResponseDTO
            {
                Token = newJwtToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                User = new CurrentUserResponseDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    AvatarUrl = _urlProvider.GetPublicUrl(user.AvatarKey)
                }

            };
        }
           


    }
}
