using Refit;
using Ruby.Shared.DTO.UserDTO;

namespace Ruby.UI.Services.Api
{
    public interface IUserApi
    {
        [Get("/api/user/me")]
        Task<CurrentUserResponseDTO> GetCurrentUserProfileAsync();

        [Get("/api/user/{id}")]
        Task<PublicUserResponseDTO> GetPublicUserProfileAsync(Guid id);

        [Put("/api/user/me")]
        Task<CurrentUserResponseDTO> UpdateProfileAsync([Body] UpdateProfileRequestDTO request);

        [Put("/api/user/me/password")]
        Task UpdatePasswordAsync([Body] UpdatePasswordRequestDTO request);
    }
}