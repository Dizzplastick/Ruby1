using Refit;
using Ruby.Shared.DTO.PlaylistDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ruby.UI.Services.Api
{
    public interface IPlaylistApi
    {
        [Get("/api/playlist/genres")]
        Task<List<PlaylistShortDTO>> GetGenrePlaylistsAsync();

        [Get("/api/playlist/latest")]
        Task<List<PlaylistShortDTO>> GetLatestPlaylistsAsync([Query] int count = 10);


        [Get("/api/playlist/{id}")]
        Task<PlaylistResponseDTO> GetPlaylistByIdAsync(Guid id);

        [Post("/api/playlist")]
        Task<PlaylistResponseDTO> CreatePlaylistAsync([Body] CreatePlaylistRequestDTO request);

        [Put("/api/playlist/{id}")]
        Task<PlaylistResponseDTO> UpdatePlaylistAsync(Guid id, [Body] UpdatePlaylistRequestDTO request);

        [Delete("/api/playlist/{id}")]
        Task DeletePlaylistAsync(Guid id);


        [Post("/api/playlist/{id}/tracks/{trackId}")]
        Task AddTrackAsync(Guid id, Guid trackId);

        [Delete("/api/playlist/{id}/tracks/{trackId}")]
        Task RemoveTrackAsync(Guid id, Guid trackId);


        [Post("/api/playlist/{id}/like")]
        Task ToggleLikeAsync(Guid id);

        [Get("/api/playlist/liked")]
        Task<List<PlaylistShortDTO>> GetLikedPlaylistsAsync();

        [Get("/api/playlist/library")]
        Task<List<PlaylistShortDTO>> GetMyLibraryAsync();

        [Get("/api/playlist/created")]
        Task<List<PlaylistShortDTO>> GetMyCreatedPlaylistsAsync();

        [Get("/api/playlist/search")]
        Task<List<PlaylistShortDTO>> SearchPlaylistsAsync([Query] string q);
    }
}