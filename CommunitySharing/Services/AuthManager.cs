using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Maui.Storage;

namespace CommunitySharing.Services
{
    public class AuthManager
    {
        private const string TokenKey = "auth_token";
        private readonly HttpClient _http;

        public string? Token { get; private set; }
        public bool IsAuthenticated => !string.IsNullOrEmpty(Token);

        public AuthManager(HttpClient http)
        {
            _http = http;
        }

        // Call this after successful login
        public async Task SaveTokenAsync(string token)
        {
            Token = token;

            // Save securely
            try
            {
                await SecureStorage.SetAsync(TokenKey, token);
            }
            catch
            {
                // fallback: Preferences (less secure)
                Preferences.Set(TokenKey, token);
            }

            AttachTokenToHttpClient(token);
        }

        public async Task<bool> LoadTokenAsync()
        {
            string? saved = null;
            try
            {
                saved = await SecureStorage.GetAsync(TokenKey);
            }
            catch { /* ignore */ }

            if (string.IsNullOrEmpty(saved))
            {
                // fallback
                saved = Preferences.Get(TokenKey, null);
            }

            if (!string.IsNullOrEmpty(saved))
            {
                Token = saved;
                AttachTokenToHttpClient(Token);
                return true;
            }

            return false;
        }

        public void AttachTokenToHttpClient(string token)
        {
            if (_http.DefaultRequestHeaders.Contains("Authorization"))
                _http.DefaultRequestHeaders.Remove("Authorization");

            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task LogoutAsync()
        {
            Token = null;
            try { SecureStorage.Remove(TokenKey); } catch { }
            Preferences.Remove(TokenKey);
            if (_http.DefaultRequestHeaders.Contains("Authorization"))
                _http.DefaultRequestHeaders.Remove("Authorization");

            // Optionally navigate to login page
        }
    }
}
