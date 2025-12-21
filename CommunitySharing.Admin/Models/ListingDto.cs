namespace CommunitySharing.Admin.Models
{
    public class ListingDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime AvailableUntil { get; set; }
    }
}
