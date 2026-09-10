using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BBL.Services;
using Ruby.Shared.DTO.LinksDTO;

namespace Ruby.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class S3StorageController : ControllerBase
    {
        private readonly IS3StorageService _s3storageService;

        public S3StorageController(IS3StorageService s3storageService)
        {
            _s3storageService = s3storageService;
        }

        [HttpPost("upload-link")]
        public async Task<IActionResult> GetUploadLink([FromBody] GenerateUploadLinkRequestDTO request)
        {
            try
            {
                var response = await _s3storageService.GeneratePresignedUploadUrlAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
    }
