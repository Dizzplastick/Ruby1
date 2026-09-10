using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ruby.UI.Services.HttpHandlers
{
    public interface ITokenStorageService
    {
        Task SaveTokensAsync(string jwt, string refreshToken);
        Task<(string Jwt, string RefreshToken)> GetTokensAsync();
        Task ClearTokensAsync();
    }
}
