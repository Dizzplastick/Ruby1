using Ruby.Shared.DTO.GenreDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Shared.DTO.TrackDTO
{
    public class TrackResponseDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public UserDTO.PublicUserShortResponseDTO Author { get; set; }

        public string AudioFileUrl { get; set; }

        public string? CoverImageUrl { get; set; }

        public int DurationSeconds { get; set; }

        public int LikesCount { get; set; }

        public bool IsLikedByCurrentUser { get; set; }

        public List<GenreResponseDTO> Genres { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
