using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ruby.Shared.DTO.LikesDTO;
using Ruby.Shared.DTO.TrackDTO;

namespace Ruby.BBL.Services
{
    public interface ITrackService
    {
        public Task<TrackResponseDTO> CreateTrackAsync(Guid currentUserId, CreateTrackRequestDTO dto);

        public Task DeleteTrackAsync(Guid currentUserId, Guid trackId);

        public Task<List<TrackShortResponseDTO>> GetLatestTracksAsync (int count = 10, Guid? currentUserId = null);
        
        public Task<TrackResponseDTO> GetTrackByIdAsync (Guid trackId, Guid currentUserId);

        public Task<TrackResponseDTO> UpdateTrackAsync (Guid currentUserId, Guid trackId, UpdateTrackRequestDTO dto);

        public Task ToggleLikeAsync (Guid currentUserId, ToggleLikeRequestDTO dto);

        Task<List<TrackShortResponseDTO>> GetLikedTracksAsync(Guid currentUserId, int? count = null);

        Task<List<TrackShortResponseDTO>> SearchTracksAsync(string query, Guid? currentUserId = null);

    }
}
