using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ruby.BBL.Services;
using Ruby.Shared.DTO.Common;
using Ruby.Shared.DTO.LikesDTO;
using Ruby.Shared.DTO.TrackDTO;
using System.Security.Claims;

namespace Ruby.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrackController : ControllerBase
    {
        private readonly ITrackService _trackService;

        public TrackController(ITrackService tracksevice)
        {
            _trackService = tracksevice;
        }

        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> CreateTrack([FromBody] CreateTrackRequestDTO request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var newTrack = await _trackService.CreateTrackAsync(userId, request);


                return CreatedAtAction(nameof(GetTrackById), new { id = newTrack.Id }, newTrack);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTrack(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                await _trackService.DeleteTrackAsync(userId, id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }


        [HttpGet("latest")]
        [Authorize]
        public async Task<IActionResult> GetLatestTracks([FromQuery] int count = 10, Guid? currentUserId = null)
        {
            var userId = GetCurrentUserId();

            try
            {
                var tracks = await _trackService.GetLatestTracksAsync(count, userId);
                return Ok(tracks);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetTrackById(Guid id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var track = await _trackService.GetTrackByIdAsync(id, currentUserId);

                return Ok(track);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTrack (Guid id, [FromBody] UpdateTrackRequestDTO request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var updatedTrack = await _trackService.UpdateTrackAsync(userId, id, request);
                return Ok(updatedTrack);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> SearchTracks([FromQuery] string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return Ok(new List<TrackShortResponseDTO>());
            }

            try
            {
                var currentUserId = GetCurrentUserId();
                var tracks = await _trackService.SearchTracksAsync(q, currentUserId);
                return Ok(tracks);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("like")]
        [Authorize] 
        public async Task<IActionResult> ToggleLike([FromBody] ToggleLikeRequestDTO request)
        {
            try
            {
                var userId = GetCurrentUserId(); 
                await _trackService.ToggleLikeAsync(userId, request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("liked")]
        [Authorize]
        public async Task<IActionResult> GetLikedTracks()
        {
            try
            {
                var userId = GetCurrentUserId();
                var tracks = await _trackService.GetLikedTracksAsync(userId);
                return Ok(tracks);
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
                throw new UnauthorizedAccessException("Invalid token or missing user ID.");
            }
            return userId;
        }
    }
}
