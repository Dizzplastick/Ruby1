using BBL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ruby.Shared.DTO.Common;
using Ruby.Shared.DTO.UserDTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Ruby.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var profile = await _userService.GetCurrentUserProfileAsync(userId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetPublicUserProfile(Guid id)
        {
            try
            {
                var currentUserId = GetCurrentUserIdOptional();

                var profile = await _userService.GetPublicUserProfileAsync(id, currentUserId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDTO request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var updatedProfile = await _userService.UpdateProfileAsync(userId, request);
                return Ok(updatedProfile); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("me/password")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequestDTO request)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _userService.UpdatePasswordAsync(userId, request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        private Guid GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(idClaim) || !Guid.TryParse(idClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid token or user ID missing.");
            }

            return userId;
        }
        private Guid? GetCurrentUserIdOptional()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(idClaim) && Guid.TryParse(idClaim, out var userId))
            {
                return userId;
            }

            try
            {
                if (Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    var token = authHeader.ToString().Replace("Bearer ", "").Trim();
                    var handler = new JwtSecurityTokenHandler();

                    if (handler.CanReadToken(token))
                    {
                        var jwtToken = handler.ReadJwtToken(token);

                        var claim = jwtToken.Claims.FirstOrDefault(c =>
                            c.Type == ClaimTypes.NameIdentifier ||
                            c.Type == "nameid" ||
                            c.Type == "sub");

                        if (claim != null && Guid.TryParse(claim.Value, out var manualUserId))
                        {
                            return manualUserId;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при ручном парсинге токена: {ex.Message}");
            }

            return null;
        }
    }
}
