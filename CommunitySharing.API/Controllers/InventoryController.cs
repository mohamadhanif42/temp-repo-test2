using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunitySharing.API.Controllers
{
    [ApiController]
    [Route("api/inventory")]
    [Authorize] // require authenticated user for reads/writes
    public class InventoryController : ControllerBase
    {
        private readonly FirestoreDb _firestore;

        public InventoryController(FirestoreDb firestore)
        {
            _firestore = firestore;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var col = _firestore.Collection("inventory");
            var snapshot = await col.GetSnapshotAsync();
            var list = new List<object>();
            foreach (var doc in snapshot.Documents)
            {
                var dict = doc.ToDictionary();
                dict["id"] = doc.Id;
                list.Add(dict);
            }
            return Ok(list);
        }

        [HttpPost("sync")]
        public async Task<IActionResult> Sync([FromBody] InventoryDto dto)
        {
            if (dto == null) return BadRequest();

            // use id if provided; otherwise create new doc with generated id
            var id = string.IsNullOrEmpty(dto.Id) ? Guid.NewGuid().ToString() : dto.Id;
            var docRef = _firestore.Collection("inventory").Document(id);

            var data = new Dictionary<string, object>
            {
                { "name", dto.Name ?? "" },
                { "description", dto.Description ?? "" },
                { "quantity", dto.Quantity },
                { "price", dto.Price },
                { "updatedAt", dto.UpdatedAt == default ? DateTime.UtcNow : dto.UpdatedAt }
            };

            await docRef.SetAsync(data, SetOptions.MergeAll);
            return Ok(new { id });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // only admin can delete in this example
        public async Task<IActionResult> Delete(string id)
        {
            var docRef = _firestore.Collection("inventory").Document(id);
            await docRef.DeleteAsync();
            return NoContent();
        }

        public class InventoryDto
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
            public string? Description { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    }
}
