using Microsoft.Extensions.DependencyInjection;
using Ruby.Shared.DTO.UserDTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Ruby.UI.Services.Api;

namespace Ruby.UI.Services.HttpHandlers
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ITokenStorageService _tokenStorage;
        private readonly IServiceProvider _serviceProvider;
        private static readonly SemaphoreSlim _refreshSemaphore = new SemaphoreSlim(1, 1);

        public AuthHeaderHandler(ITokenStorageService tokenStorage, IServiceProvider serviceProvider)
        {
            _tokenStorage = tokenStorage;
            _serviceProvider = serviceProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var tokens = await _tokenStorage.GetTokensAsync();

            if (!string.IsNullOrEmpty(tokens.Jwt))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.Jwt);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(tokens.RefreshToken))
            {
                await _refreshSemaphore.WaitAsync(cancellationToken);
                try
                {
                    var currentTokens = await _tokenStorage.GetTokensAsync();
                    if (currentTokens.Jwt != tokens.Jwt)
                    {
                        using var newRequest = await CloneHttpRequestMessageAsync(request);
                        newRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", currentTokens.Jwt);
                        return await base.SendAsync(newRequest, cancellationToken);
                    }

                    bool refreshed = await TryRefreshTokenAsync(tokens.RefreshToken);

                    if (refreshed)
                    {
                        var newTokens = await _tokenStorage.GetTokensAsync();
                        using var newRequest = await CloneHttpRequestMessageAsync(request);
                        newRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newTokens.Jwt);
                        return await base.SendAsync(newRequest, cancellationToken);
                    }
                    else
                    {
                        await _tokenStorage.ClearTokensAsync();
                    }
                }
                finally
                {
                    _refreshSemaphore.Release();
                }
            }

            return response;
        }

        private async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Version = request.Version
            };

            if (request.Content != null)
            {
                var ms = new MemoryStream();
                await request.Content.CopyToAsync(ms);
                ms.Position = 0;
                clone.Content = new StreamContent(ms);

                foreach (var header in request.Content.Headers)
                {
                    clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            foreach (var header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
        }


        private async Task<bool> TryRefreshTokenAsync(string refreshToken)
        {
            try
            {
                var authApi = _serviceProvider.GetRequiredService<IAuthApi>();

                var request = new RefreshJWTRequestDTO { RefreshToken = refreshToken };
                var authResponse = await authApi.RefreshTokenAsync(request);

                if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
                {
                    await _tokenStorage.SaveTokensAsync(authResponse.Token, authResponse.RefreshToken);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }


    }
}
