using Ruby.Shared.DTO.LinksDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBL.Services
{
    public interface IS3StorageService
    {
        Task<GenerateUploadLinkResponseDTO> GeneratePresignedUploadUrlAsync(GenerateUploadLinkRequestDTO dto);

        Task DeleteFileAsync(string fileKey);
    }
}
