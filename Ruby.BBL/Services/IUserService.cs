using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ruby.Shared.DTO.UserDTO;

namespace BBL.Services
{
    public interface IUserService
    {
        public Task<CurrentUserResponseDTO> GetCurrentUserProfileAsync(Guid currentUserId);

        public Task<PublicUserResponseDTO> GetPublicUserProfileAsync(Guid userId, Guid? currentUserId);

        public Task UpdatePasswordAsync (Guid currentUserId, UpdatePasswordRequestDTO dto);

        public Task<CurrentUserResponseDTO> UpdateProfileAsync(Guid currentUserId, UpdateProfileRequestDTO dto);
    }
}
