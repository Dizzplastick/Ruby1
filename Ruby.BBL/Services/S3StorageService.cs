using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Ruby.Shared.Enums;
using Ruby.Shared.DTO.LinksDTO;
using BBL.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBL.Services
{
    public class S3StorageService : IS3StorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3StorageOptions _options;
        private readonly string _publicBaseUrl;


        public S3StorageService(IAmazonS3 s3Client, IOptions<S3StorageOptions> options, IOptions<StorageOptions> publicUrlOptions)
        {
            _s3Client = s3Client;
            _options = options.Value;
            _publicBaseUrl = publicUrlOptions.Value.BaseUrl;
        }


        public async Task<GenerateUploadLinkResponseDTO> GeneratePresignedUploadUrlAsync(GenerateUploadLinkRequestDTO request)
        {

            long maxBytes = request.FileType == UploadFileTypeEnum.TrackAudio ? 35_000_000 : 8_000_000; // 35 МБ для аудiо, 8 МБ для фото
            if (request.FileSize > maxBytes)
                throw new Exception($"File is too large. Max is: {maxBytes / 1000000} Mb.");


            string extension = Path.GetExtension(request.FileName).ToLower();


            string folder = request.FileType switch
            {
                UploadFileTypeEnum.TrackAudio => "tracks",
                UploadFileTypeEnum.TrackCover => "covers",
                UploadFileTypeEnum.UserProfileAvatar => "avatars",
                _ => throw new ArgumentOutOfRangeException(nameof(request.FileType), $"Unexpected file type: {request.FileType}")
            };

            string contentType = request.FileType switch
            {
                UploadFileTypeEnum.TrackAudio => extension switch
                {
                    ".mp3" => "audio/mpeg",
                    ".wav" => "audio/wav",
                    ".flac" => "audio/flac",
                    ".m4a" => "audio/mp4",
                    _ => throw new Exception($"Unsupported audio format: {extension}. Use mp3, wav, flac or m4a.")
                },
                UploadFileTypeEnum.TrackCover or UploadFileTypeEnum.UserProfileAvatar => extension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".webp" => "image/webp",
                    _ => throw new Exception($"Unsupported image format: {extension}. Use jpg, png or webp.")
                },
                _ => throw new Exception("Unknown file type")
            };


            string fileKey = $"{folder}/{Guid.NewGuid()}{extension}";


            var s3Request = new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = fileKey,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(15),
                ContentType = contentType,
                Protocol = _options.ServiceUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase)
               ? Protocol.HTTPS
               : Protocol.HTTP
            };


            string presignedUrl = await _s3Client.GetPreSignedURLAsync(s3Request);


            return new GenerateUploadLinkResponseDTO
            {
                PresignedUrl = presignedUrl,
                FileKey = fileKey,
                PublicUrl = $"{_publicBaseUrl.TrimEnd('/')}/{fileKey}"
            };
        }

        public async Task DeleteFileAsync(string fileKey)
        {
            if (string.IsNullOrWhiteSpace(fileKey)) return;

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = fileKey
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);

        }
    }
}
