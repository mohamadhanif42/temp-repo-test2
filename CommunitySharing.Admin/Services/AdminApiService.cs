using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Diagnostics;

namespace CommunitySharing.Admin.Services
{
    public class AdminApiService
    {
        private readonly HttpClient _http;
        private readonly ProtectedSessionStorage _session;
        private readonly JsonSerializerOptions _jsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        // API base URL (change to your API address)
        private const string ApiBase = "https://localhost:7080";
        // Firebase Web API Key
        private const string FirebaseWebApiKey = "AIzaSyClaxO656Jgm2vcUAKRWHhABIcgCWkTldk";

        public AdminApiService(HttpClient http, ProtectedSessionStorage session)
        {
            _http = http;
            _session = session;
        }

        // Persist the app JWT in protected session storage and set header
        public async Task SaveAppTokenAsync(string token)
        {
            await _session.SetAsync("app_token", token);
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<string?> GetSavedTokenAsync()
        {
            var result = await _session.GetAsync<string>("app_token");
            if (result.Success && !string.IsNullOrEmpty(result.Value))
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Value);
                return result.Value;
            }
            return null;
        }

        public async Task ClearTokenAsync()
        {
            await _session.DeleteAsync("app_token");
            _http.DefaultRequestHeaders.Authorization = null;
        }

        // Firebase sign-in (REST) -> returns idToken
        public async Task<(bool Ok, string? IdToken, string? Error)> FirebaseSignInAsync(string email, string password)
        {
            try
            {
                var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={FirebaseWebApiKey}";
                var payload = new { email, password, returnSecureToken = true };
                var resp = await _http.PostAsJsonAsync(url, payload);
                if (!resp.IsSuccessStatusCode)
                {
                    var txt = await resp.Content.ReadAsStringAsync();
                    return (false, null, txt);
                }

                var body = await resp.Content.ReadFromJsonAsync<FirebaseAuthResponse>(_jsonOpts);
                return (true, body?.IdToken, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return (false, null, ex.Message);
            }
        }

        // Exchange idToken at your API -> returns app token
        public async Task<(bool Ok, string? AppToken, string? Error)> ExchangeIdTokenAsync(string idToken)
        {
            try
            {
                var resp = await _http.PostAsJsonAsync($"{ApiBase}/api/auth/login", new { idToken }, _jsonOpts);
                if (!resp.IsSuccessStatusCode)
                {
                    var txt = await resp.Content.ReadAsStringAsync();
                    return (false, null, txt);
                }

                var data = await resp.Content.ReadFromJsonAsync<ExchangeResponse>(_jsonOpts);
                if (data == null || string.IsNullOrEmpty(data.Token))
                    return (false, null, "No token returned");

                // set header
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", data.Token);
                await SaveAppTokenAsync(data.Token);

                return (true, data.Token, null);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        // Inventory endpoints
        public async Task<List<InventoryDto>?> GetInventoryAsync()
        {
            var resp = await _http.GetAsync($"{ApiBase}/api/inventory");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<List<InventoryDto>>();
        }

        public async Task<bool> CreateOrUpdateInventoryAsync(InventoryDto dto)
        {
            var resp = await _http.PostAsJsonAsync($"{ApiBase}/api/inventory/sync", dto);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteInventoryAsync(string id)
        {
            var resp = await _http.DeleteAsync($"{ApiBase}/api/inventory/{id}");
            return resp.IsSuccessStatusCode;
        }

        // Users (admin-only)
        public async Task<List<UserDto>?> GetUsersAsync()
        {
            var resp = await _http.GetAsync($"{ApiBase}/api/admin/users");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<List<UserDto>>();
        }

        public async Task<bool> SetUserRoleAsync(int id, string role)
        {
            var resp = await _http.PutAsJsonAsync($"{ApiBase}/api/admin/users/{id}/role", role);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var resp = await _http.DeleteAsync($"{ApiBase}/api/admin/users/{id}");
            return resp.IsSuccessStatusCode;
        }

        // DTOs
        private class FirebaseAuthResponse
        {
            public string IdToken { get; set; }
            public string RefreshToken { get; set; }
            public string ExpiresIn { get; set; }
            public string LocalId { get; set; }
        }

        private class ExchangeResponse
        {
            public string Token { get; set; }
            public string Role { get; set; }
        }

        public class InventoryDto
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string? Description { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class UserDto
        {
            public int Id { get; set; }
            public string Email { get; set; }
            public string? Role { get; set; }
            public DateTime? CreatedAt { get; set; }
        }
    }
}
