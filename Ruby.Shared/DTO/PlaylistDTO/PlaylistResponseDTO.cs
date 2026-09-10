using Ruby.Shared.DTO.TrackDTO;
using Ruby.Shared.DTO.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Shared.DTO.PlaylistDTO
{
    public class PlaylistResponseDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string? CoverImageUrl { get; set; }

        public bool IsPrivate { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsLikedByCurrentUser { get; set; }

        public PublicUserShortResponseDTO Author { get; set; }

        public List<TrackShortResponseDTO> Tracks { get; set; }

        
    }
}
