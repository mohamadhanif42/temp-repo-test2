namespace CommunitySharing.Models
{
    public class SharedItem
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Distance { get; set; } // e.g., "2 KM"
        public int StockAvailable { get; set; }
    }
}
