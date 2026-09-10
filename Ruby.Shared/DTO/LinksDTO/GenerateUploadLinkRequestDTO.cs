using Ruby.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.Shared.DTO.LinksDTO
{
    public class GenerateUploadLinkRequestDTO
    {
        public string FileName { get; set; } 

        public long FileSize { get; set; }

        public UploadFileTypeEnum FileType { get; set; } 


    }
}
