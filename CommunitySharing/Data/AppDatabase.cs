using SQLite;
using System.IO;

namespace CommunitySharing.Data
{
    public class AppDatabase
    {
        private readonly SQLiteAsyncConnection _db;

        public AppDatabase(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
            _db.CreateTableAsync<UserSession>().Wait();
            _db.CreateTableAsync<LocalInventoryItem>().Wait();
        }

        // Session
        public Task<UserSession?> GetSessionAsync() =>
            _db.Table<UserSession>().FirstOrDefaultAsync();

        public Task SaveSessionAsync(UserSession session) =>
            _db.InsertOrReplaceAsync(session);

        public Task ClearSessionAsync() => _db.DeleteAllAsync<UserSession>();

        // Inventory
        public Task<List<LocalInventoryItem>> GetInventoryAsync() =>
            _db.Table<LocalInventoryItem>().ToListAsync();

        public Task<List<LocalInventoryItem>> GetUnsyncedAsync() =>
            _db.Table<LocalInventoryItem>().Where(i => i.IsSynced == false).ToListAsync();

        public Task SaveInventoryAsync(LocalInventoryItem item)
        {
            item.UpdatedAt = DateTime.UtcNow; // update timestamp
            // note: InsertOrReplaceAsync will insert or replace, ensure PK is same
            return _db.InsertOrReplaceAsync(item);
        }

        public Task DeleteInventoryAsync(LocalInventoryItem item) =>
            _db.DeleteAsync(item);
    }
}
