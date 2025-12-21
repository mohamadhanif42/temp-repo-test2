using System.Net.Http.Json;
using CommunitySharing.Models;

public class ListingService
{
    private readonly HttpClient _http;

    public ListingService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ListingModel>> GetListingsAsync()
    {
        return await _http.GetFromJsonAsync<List<ListingModel>>("api/listings")
               ?? new();
    }
}
