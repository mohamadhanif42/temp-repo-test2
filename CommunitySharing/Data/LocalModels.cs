using SQLite;

namespace CommunitySharing.Data
{
    public class UserSession
    {
        [PrimaryKey]
        public int Id { get; set; } = 1;
        public string Token { get; set; } = "";
        public string Email { get; set; } = "";
    }

    public class LocalInventoryItem
    {
        [PrimaryKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool IsSynced { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class CloudInventoryItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool IsSynced { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
