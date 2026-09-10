using Ruby.Shared.DTO.PlaylistDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.BBL.Services
{
    public interface IPlaylistService
    {
        Task<PlaylistResponseDTO> CreatePlaylistAsync(Guid currentUserId, CreatePlaylistRequestDTO dto);

        Task<PlaylistResponseDTO> GetPlaylistByIdAsync(Guid playlistId, Guid? currentUserId = null);

        Task<PlaylistResponseDTO> UpdatePlaylistAsync(Guid currentUserId, Guid playlistId, UpdatePlaylistRequestDTO dto);

        Task DeletePlaylistAsync(Guid currentUserId, Guid playlistId);
        
        Task AddTrackToPlaylistAsync(Guid currentUserId, Guid playlistId, Guid trackId);

        Task RemoveTrackFromPlaylistAsync(Guid currentUserId, Guid playlistId, Guid trackId);

        Task<List<PlaylistShortDTO>> GetMyLibraryAsync(Guid currentUserId);

        Task<List<PlaylistShortDTO>> GetMyCreatedPlaylistsAsync(Guid currentUserId);

        Task<List<PlaylistShortDTO>> GetLatestPlaylistsAsync(int count, Guid? userId );

        Task<List<PlaylistShortDTO>> GetPlaylistsByIdsAsync(List<Guid> ids, Guid? userId);

        Task ToggleLikeAsync(Guid playlistId,Guid currentUserId );

        Task<List<PlaylistShortDTO>> GetLikedPlaylistsAsync(Guid currentUserId);

        Task<List<PlaylistShortDTO>> SearchPlaylistsAsync(string query, Guid? currentUserId = null);
    }
}
