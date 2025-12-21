namespace CommunitySharing.Admin.Models
{
    public class UserModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "user";
        public DateTime? CreatedAt { get; set; } // <--- MUST BE NULLABLE
    }
}
