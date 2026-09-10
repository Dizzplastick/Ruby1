using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Shared.DTO.UserDTO
{
    public class PublicUserResponseDTO
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string? AvatarUrl { get; set; }

        public List<TrackDTO.TrackShortResponseDTO>? Tracks { get; set; }

        public List<PlaylistDTO.PlaylistShortDTO>? Playlists { get; set; }

        
    }
}
