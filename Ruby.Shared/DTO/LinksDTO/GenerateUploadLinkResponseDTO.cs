using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Shared.DTO.LinksDTO
{
    public class GenerateUploadLinkResponseDTO
    {
        public string PresignedUrl { get; set; } 

        public string FileKey { get; set; }

        public string? PublicUrl { get; set; }
    }
}
