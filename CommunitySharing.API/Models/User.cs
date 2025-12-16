using Google.Cloud.Firestore;

namespace CommunitySharing.API.Models
{
    [FirestoreData]
    public record User
    {
        public string Id { get; set; }  // Firestore document ID

        [FirestoreProperty]
        public string Email { get; set; }

        [FirestoreProperty]
        public string Role { get; set; } = "user";

        [FirestoreProperty]
        public DateTime? CreatedAt { get; set; }
    }
}

