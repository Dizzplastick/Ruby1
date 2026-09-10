using Ruby.Shared.DTO.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Shared.DTO.TrackDTO
{
    public class UpdateTrackRequestDTO
    {
        public Guid Id { get; set; }

        public string? Title { get; set; }

       public List<int>? GenreIds { get; set; }

        public string? CoverImageKey { get; set; }

    }
}
