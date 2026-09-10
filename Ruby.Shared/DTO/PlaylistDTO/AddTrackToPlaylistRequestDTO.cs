using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Shared.DTO.PlaylistDTO
{
    public class AddTrackToPlaylistRequestDTO
    {
        public Guid TrackId { get; set; }

        public Guid PlaylistId { get; set; }
    }
}
