using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Shared.DTO.PlaylistDTO
{
    public class RemoveTrackFromPlaylistRequestDTO
    {
        public Guid TrackId { get; set; }

        public Guid PlaylistId { get; set; }
    }
}
