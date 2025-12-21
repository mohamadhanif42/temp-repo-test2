using CommunitySharing.Data;
using System.Diagnostics;

namespace CommunitySharing.Services
{
    public class SyncManager
    {
        private readonly AppDatabase _localDb;
        private readonly ApiService _api;

        public SyncManager(AppDatabase localDb, ApiService api)
        {
            _localDb = localDb;
            _api = api;
        }

        // Pull latest from cloud and store locally
        public async Task SyncFromCloudAsync()
        {
            var cloud = await _api.GetInventoryFromCloudAsync();
            if (cloud == null) return;

            foreach (var c in cloud)
            {
                var local = new LocalInventoryItem
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Quantity = c.Quantity,
                    Price = c.Price,
                    IsSynced = true,
                    UpdatedAt = c.UpdatedAt
                };
                await _localDb.SaveInventoryAsync(local);
            }
        }

        // Push unsynced local items to cloud
        public async Task SyncToCloudAsync()
        {
            var unsynced = await _localDb.GetUnsyncedAsync();
            foreach (var item in unsynced)
            {
                var (ok, err) = await _api.SyncItemToCloudAsync(item);
                if (ok)
                {
                    item.IsSynced = true;
                    await _localDb.SaveInventoryAsync(item);
                }
                else
                {
                    Debug.WriteLine($"Failed to sync item {item.Id}: {err}");
                }
            }
        }
    }
}
