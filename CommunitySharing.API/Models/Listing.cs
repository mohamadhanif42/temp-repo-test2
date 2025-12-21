using Google.Cloud.Firestore;

namespace CommunitySharing.API.Models
{
    [FirestoreData] // 🔥 REQUIRED
    public class Listing
    {
        [FirestoreProperty]
        public string Id { get; set; } = "";

        [FirestoreProperty]
        public string Title { get; set; } = "";

        [FirestoreProperty]
        public string Category { get; set; } = "";

        [FirestoreProperty]
        public DateTime AvailableUntil { get; set; }

        [FirestoreProperty]
        public string ImageBase64 { get; set; } = "";

        [FirestoreProperty]
        public string OwnerEmail { get; set; } = "";

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

