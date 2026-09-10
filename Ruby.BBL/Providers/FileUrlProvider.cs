using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBL.Options;


namespace BBL.Providers
{
    public class FileUrlProvider : IFileUrlProvider
    {
        private readonly string _baseUrl;

        public FileUrlProvider(IOptions<StorageOptions> options)
        {
            _baseUrl = options.Value.BaseUrl;
        }

        public string? GetPublicUrl(string? fileKey)
        {
            if (string.IsNullOrWhiteSpace(fileKey))
                return null;
            return $"{_baseUrl.TrimEnd('/')}/{fileKey.TrimStart('/')}";
        }
    }
}
