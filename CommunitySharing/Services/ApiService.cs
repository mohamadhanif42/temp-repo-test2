using CommunitySharing.Data;
using System.Buffers.Text;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace CommunitySharing.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ApiService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri(ApiConfig.BaseUrl) // <- use centralized URL
            };
        }

        // Firebase web API key (from Firebase console)
        private const string FirebaseWebApiKey = "AIzaSyClaxO656Jgm2vcUAKRWHhABIcgCWkTldk";

        // Sign in with Firebase via REST -> get idToken
        public async Task<(bool Success, string? IdToken, string? Error)> FirebaseSignInAsync(string email, string password)
        {
            try
            {
                var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={FirebaseWebApiKey}";
                var payload = new { email = email, password = password, returnSecureToken = true };
                var resp = await _http.PostAsJsonAsync(url, payload);
                if (!resp.IsSuccessStatusCode)
                {
                    var text = await resp.Content.ReadAsStringAsync();
                    return (false, null, text);
                }
                var body = await resp.Content.ReadFromJsonAsync<FirebaseAuthResponse>(_jsonOptions);
                return (true, body?.idToken, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("FirebaseSignInAsync ex: " + ex);
                return (false, null, ex.Message);
            }
        }

        // Exchange idToken with API to get your app JWT
        public async Task<(bool Success, string? AppToken, string? Error)> ExchangeFirebaseIdTokenAsync(string idToken)
        {
            try
            {
                var resp = await _http.PostAsJsonAsync($"{ApiConfig.BaseUrl}/api/auth/login", new { idToken }, _jsonOptions);
                if (!resp.IsSuccessStatusCode)
                {
                    var txt = await resp.Content.ReadAsStringAsync();
                    return (false, null, txt);
                }
                var data = await resp.Content.ReadFromJsonAsync<ExchangeResponse>(_jsonOptions);
                return (true, data?.Token, null);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        // ================= Inventory Sync =================

        // Fetch inventory from cloud
        public async Task<List<CloudInventoryItem>> GetInventoryFromCloudAsync()
        {
            try
            {
                var list = await _http.GetFromJsonAsync<List<CloudInventoryItem>>("/api/inventory");
                return list ?? new List<CloudInventoryItem>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("GetInventoryFromCloudAsync ex: " + ex);
                return new List<CloudInventoryItem>();
            }
        }

        // Push a single local item to cloud
        public async Task<(bool, string)> SyncItemToCloudAsync(LocalInventoryItem item)
        {
            try
            {
                var resp = await _http.PostAsJsonAsync("/api/inventory", item);
                if (resp.IsSuccessStatusCode)
                    return (true, string.Empty);
                else
                {
                    var txt = await resp.Content.ReadAsStringAsync();
                    return (false, txt);
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // DTOs
        public class FirebaseAuthResponse
        {
            public string idToken { get; set; }
            public string refreshToken { get; set; }
            public string expiresIn { get; set; }
            public string localId { get; set; }
        }

        public class ExchangeResponse
        {
            public string Token { get; set; }
            public string Role { get; set; }
        }

        
    }
}
