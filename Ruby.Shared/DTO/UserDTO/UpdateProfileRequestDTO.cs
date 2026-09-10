using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Shared.DTO.UserDTO
{
    public class UpdateProfileRequestDTO
    {
        public string? Username { get; set; }

        public string? AvatarKey { get; set; }

        public string? Email { get; set; }
        
    }
}
