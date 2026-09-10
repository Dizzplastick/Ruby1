using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Shared.DTO.TrackDTO
{
    public class CreateTrackRequestDTO
    {
        public string Title { get; set; }

        public List<int>? GenreIds { get; set; }

        public string AudioFileKey { get; set; }

        public string? CoverImageKey { get; set; }

        public int DurationSeconds { get; set; }
    }
}
