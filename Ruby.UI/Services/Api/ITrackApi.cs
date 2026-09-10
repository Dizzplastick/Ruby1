using Refit;
using Ruby.Shared.DTO.LikesDTO;
using Ruby.Shared.DTO.PlaylistDTO;
using Ruby.Shared.DTO.TrackDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.UI.Services.Api
{
  public interface ITrackApi
        {
        [Get("/api/track/latest")]
        Task<List<TrackShortResponseDTO>> GetLatestTracksAsync([Query] int count = 10);

        [Get("/api/track/{id}")]
        Task<TrackResponseDTO> GetTrackByIdAsync(Guid id);

        [Post("/api/track")]
        Task<TrackResponseDTO> CreateTrackAsync([Body] CreateTrackRequestDTO request);

        [Put("/api/track/{id}")]
        Task<TrackResponseDTO> UpdateTrackAsync(Guid id, [Body] UpdateTrackRequestDTO request);

        [Delete("/api/track/{id}")]
        Task DeleteTrackAsync(Guid id);


        [Post("/api/track/like")]
        Task ToggleLikeAsync([Body] ToggleLikeRequestDTO request);

        [Get("/api/track/liked")]
        Task<List<TrackShortResponseDTO>> GetLikedTracksAsync([Query] int count = 12);

        [Get("/api/track/search")]
        Task<List<TrackShortResponseDTO>> SearchTracksAsync([Query] string q);

        [Get("/api/playlist/search")]
        Task<List<PlaylistShortDTO>> SearchPlaylistsAsync([Query] string q);

        
    }
}
