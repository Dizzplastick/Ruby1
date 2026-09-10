using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Shared.DTO.UserDTO
{
    public class CurrentUserResponseDTO
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string? Email { get; set; }

        public string? AvatarUrl { get; set; }

        public DateTime CreatedAt { get; set; }


    }
}
