using Microsoft.JSInterop;
using Ruby.UI.Services.HttpHandlers;

namespace Ruby.Web.Services
{
    public class WebTokenStorageService : ITokenStorageService
    {
        private readonly IJSRuntime _jsRuntime;

        public WebTokenStorageService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SaveTokensAsync(string jwt, string refreshToken)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "jwt", jwt);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "refreshToken", refreshToken);
        }

        public async Task<(string Jwt, string RefreshToken)> GetTokensAsync()
        {
            var jwt = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwt");
            var refreshToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "refreshToken");

            return (jwt, refreshToken);
        }

        public async Task ClearTokensAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "jwt");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "refreshToken");
        }
    }
}
