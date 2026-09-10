using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Shared.DTO.PlaylistDTO
{
    public class UpdatePlaylistRequestDTO
    {
        public Guid Id { get; set; }

        public string? Title { get; set; }
        
        public string? CoverImageKey { get; set; }

        public bool? IsPrivate { get; set; }
    }
}
