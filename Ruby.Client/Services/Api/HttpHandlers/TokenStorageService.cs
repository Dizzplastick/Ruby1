using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Ruby.UI.Services.HttpHandlers;

namespace Ruby.Client.Services.Api.HttpHandlers
{
    public class TokenStorageService : ITokenStorageService
    {
        private readonly string _storagePath;
        private readonly string _fileName = "auth_tokens.json";

        private string _cachedJwt;
        private string _cachedRefresh;

        public TokenStorageService()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _storagePath = Path.Combine(appData, "Ruby");

            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public async Task SaveTokensAsync(string jwt, string refreshToken)
        {
            _cachedJwt = jwt;
            _cachedRefresh = refreshToken;

            var tokens = new { Jwt = jwt, RefreshToken = refreshToken };
            var json = JsonSerializer.Serialize(tokens);
            await File.WriteAllTextAsync(Path.Combine(_storagePath, _fileName), json);
        }

        public async Task<(string Jwt, string RefreshToken)> GetTokensAsync()
        {
            if (!string.IsNullOrEmpty(_cachedJwt))
            {
                return (_cachedJwt, _cachedRefresh);
            }

            var filePath = Path.Combine(_storagePath, _fileName);
            if (!File.Exists(filePath)) return (null, null);

            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                var tokens = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                _cachedJwt = tokens?["Jwt"];
                _cachedRefresh = tokens?["RefreshToken"];

                return (_cachedJwt, _cachedRefresh);
            }
            catch
            {
                return (null, null);
            }
        }

        public async Task ClearTokensAsync()
        {
            _cachedJwt = null;
            _cachedRefresh = null;

            var filePath = Path.Combine(_storagePath, _fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            await Task.CompletedTask;
        }
    }
}