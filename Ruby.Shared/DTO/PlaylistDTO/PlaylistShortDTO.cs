using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Shared.DTO.PlaylistDTO
{
    public class PlaylistShortDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? CoverImageUrl { get; set; }
        public int TracksCount { get; set; }

        public bool IsLikedByCurrentUser { get; set; }
    }
}
