using Google.Cloud.Firestore;
using CommunitySharing.API.Models;

namespace CommunitySharing.API.Services
{
    public class UserService
    {
        private readonly FirestoreDb _db;
        private const string Collection = "users";

        public UserService(FirestoreDb db)
        {
            _db = db;
        }

        // 🔥 Create new user and save to Firestore
        public async Task AddUserAsync(User user)
        {
            user.CreatedAt = DateTime.UtcNow;
            user.Role = "user"; // default user
            await _db.Collection(Collection).AddAsync(user);
        }

        // 🔥 Get user by email (for login)
        public async Task<User?> GetByEmailAsync(string email)
        {
            var query = _db.Collection(Collection).WhereEqualTo("Email", email);
            var results = await query.GetSnapshotAsync();

            if (!results.Any()) return null;

            return results.First().ConvertTo<User>() with { Id = results.First().Id };
        }

        // 🔥 Admin – Get ALL users
        public async Task<List<User>> GetAllUsersAsync()
        {
            var snapshot = await _db.Collection(Collection).GetSnapshotAsync();
            var users = new List<User>();

            foreach (var doc in snapshot.Documents)
            {
                var user = doc.ConvertTo<User>();
                user.Id = doc.Id; // attach Firestore ID

                // Ensure CreatedAt never breaks Blazor
                user.CreatedAt ??= null;

                users.Add(user);
            }

            return users;
        }

        // 🔥 Admin – Change Role
        public async Task<bool> UpdateRoleAsync(string userId, string role)
        {
            var userRef = _db.Collection(Collection).Document(userId);
            await userRef.UpdateAsync("Role", role);
            return true;
        }

        // 🔥 Admin – Delete User
        public async Task<bool> DeleteAsync(string userId)
        {
            await _db.Collection(Collection).Document(userId).DeleteAsync();
            return true;
        }
    }
}
