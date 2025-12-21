using CommunitySharing.Admin.Models;
using System.Net.Http.Json;

public class ListingService
{
    private readonly HttpClient _http;

    public ListingService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ListingDto>> GetListingsAsync()
    {
        return await _http.GetFromJsonAsync<List<ListingDto>>("api/listings")
               ?? new List<ListingDto>();
    }
}
