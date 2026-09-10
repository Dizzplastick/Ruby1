using Refit;
using Ruby.Shared.DTO.LinksDTO; 

namespace Ruby.UI.Services.Api
{
    public interface IStorageApi
    {
        [Post("/api/S3Storage/upload-link")] 
        Task<GenerateUploadLinkResponseDTO> GenerateUploadLinkAsync([Body] GenerateUploadLinkRequestDTO request);
    }
}