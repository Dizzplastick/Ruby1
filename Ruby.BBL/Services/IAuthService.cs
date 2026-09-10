using Ruby.Shared.DTO.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBL.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(UserRegisterRequestDTO dto);

        Task<AuthResponseDTO> LoginAsync(UserLogInRequestDTO dto);

        Task<AuthResponseDTO> RefreshTokenAsync(RefreshJWTRequestDTO dto);  

    }
}
