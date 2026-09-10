using Ruby.Shared.DTO.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace Ruby.UI.Services.Api
{
    public interface IAuthApi
    {
        [Post("/api/auth/register")]
        Task<AuthResponseDTO> RegisterAsync([Body] UserRegisterRequestDTO request);

        [Post("/api/auth/login")]
        Task<AuthResponseDTO> LoginAsync([Body] UserLogInRequestDTO request);

        [Post("/api/auth/refreshJWT")]
        Task<AuthResponseDTO> RefreshTokenAsync([Body] RefreshJWTRequestDTO request);

    }
}
