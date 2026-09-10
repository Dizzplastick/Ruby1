using Microsoft.AspNetCore.Mvc;
using BBL.Services;
using Ruby.Shared.DTO.Common;
using Ruby.Shared.DTO.UserDTO;
using System.Linq.Expressions;

namespace Ruby.Server.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserRegisterRequestDTO request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiErrorResponseDTO
                {
                    StatusCode = 400,
                    Message = ex.Message
                });
            }
        }

            [HttpPost("login")]
            public async Task<IActionResult> Login([FromBody] UserLogInRequestDTO request)
            {
                try
                {
                    var response = await _authService.LoginAsync(request);
                    return Ok(response);
                }
                catch (Exception ex) 
                {
                    return BadRequest(new ApiErrorResponseDTO
                    {
                        StatusCode = 400,
                        Message = ex.Message
                    });
                }
            }

        [HttpPost("refreshJWT")]
        public async Task<IActionResult> refreshJWT([FromBody] RefreshJWTRequestDTO request) 
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(new ApiErrorResponseDTO
                {
                    StatusCode = 401,
                    Message = ex.Message
                });
            }


        }


        }
    }

