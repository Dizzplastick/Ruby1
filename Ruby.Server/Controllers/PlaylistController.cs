using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ruby.BBL.Services;
using Ruby.Shared.DTO.PlaylistDTO;
using System.Security.Claims;

namespace Ruby.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;
        private readonly IConfiguration _configuration;

        public PlaylistController(IPlaylistService playlistService, IConfiguration configuration)
        {
            _playlistService = playlistService;
            _configuration = configuration;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistRequestDTO request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var playlist = await _playlistService.CreatePlaylistAsync(userId, request);
                return CreatedAtAction(nameof(GetPlaylistById), new { id = playlist.Id }, playlist);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetPlaylistById(Guid id)
        {
            try
            {
                var currentUserId = GetCurrentUserIdOptional();
                var playlist = await _playlistService.GetPlaylistByIdAsync(id, currentUserId);
                return Ok(playlist);
            }
            catch (Exception ex) { return NotFound(new { Message = ex.Message }); }
        }

        [HttpGet("latest")]
        [Authorize]
        public async Task<IActionResult> GetLatestPlaylists([FromQuery] int count = 10)
        {
            try
            {
                var userId = GetCurrentUserIdOptional();

                var playlists = await _playlistService.GetLatestPlaylistsAsync(count, userId);
                return Ok(playlists);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("genres")]
        [Authorize]
        public async Task<IActionResult> GetGenrePlaylists()
        {
            try
            {
                var userId = GetCurrentUserIdOptional();
                var genreIds = _configuration.GetSection("GenrePlaylistIds").Get<List<Guid>>() ?? new List<Guid>();

                if (!genreIds.Any()) return Ok(new List<PlaylistShortDTO>());

                var playlists = await _playlistService.GetPlaylistsByIdsAsync(genreIds, userId);

                return Ok(playlists);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePlaylist(Guid id, [FromBody] UpdatePlaylistRequestDTO request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var playlist = await _playlistService.UpdatePlaylistAsync(userId, id, request);
                return Ok(playlist);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePlaylist(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _playlistService.DeletePlaylistAsync(userId, id);
                return NoContent();
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }


        [HttpPost("{id}/tracks/{trackId}")]
        [Authorize]
        public async Task<IActionResult> AddсTrack(Guid id, Guid trackId)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _playlistService.AddTrackToPlaylistAsync(userId, id, trackId);
                return Ok();
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        [HttpDelete("{id}/tracks/{trackId}")]
        [Authorize]
        public async Task<IActionResult> RemoveTrack(Guid id, Guid trackId)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _playlistService.RemoveTrackFromPlaylistAsync(userId, id, trackId);
                return NoContent();
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }


        [HttpPost("{id}/like")]
        [Authorize]
        public async Task<IActionResult> ToggleLike(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _playlistService.ToggleLikeAsync(id, userId );
                return Ok();
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        [HttpGet("liked")]
        [Authorize]
        public async Task<IActionResult> GetLikedPlaylists()
        {
            try
            {
                var userId = GetCurrentUserId();
                var playlists = await _playlistService.GetLikedPlaylistsAsync(userId);
                return Ok(playlists);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        [HttpGet("library")]
        [Authorize]
        public async Task<IActionResult> GetMyLibrary()
        {
            try
            {
                var userId = GetCurrentUserId();
                var library = await _playlistService.GetMyLibraryAsync(userId);
                return Ok(library);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("created")]
        [Authorize]
        public async Task<IActionResult> GetMyCreatedPlaylists()
        {
            try
            {
                var userId = GetCurrentUserId();
                var playlists = await _playlistService.GetMyCreatedPlaylistsAsync(userId);
                return Ok(playlists);
            }
            catch (Exception ex) { return BadRequest(new { Message = ex.Message }); }
        }

        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> SearchPlaylists([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return Ok(new List<PlaylistShortDTO>());
            }

            try
            {
                var currentUserId = GetCurrentUserIdOptional();
                var playlists = await _playlistService.SearchPlaylistsAsync(q, currentUserId);
                return Ok(playlists);
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
                throw new UnauthorizedAccessException();
            return userId;
        }

        private Guid? GetCurrentUserIdOptional()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(idClaim) && Guid.TryParse(idClaim, out var userId))
                return userId;
            return null;
        }
    }
}