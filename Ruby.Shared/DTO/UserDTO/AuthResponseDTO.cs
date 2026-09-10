using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Shared.DTO.UserDTO
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }

        public DateTime ExpiresAt { get; set; }

        public string RefreshToken { get; set; }

        public CurrentUserResponseDTO User { get; set; }

    }
}
