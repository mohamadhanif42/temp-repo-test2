using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using CommunitySharing.API.Models;

namespace CommunitySharing.API.Controllers
{
    [ApiController]
    [Route("api/listings")]
    public class ListingsController : ControllerBase
    {
        private readonly FirestoreDb _db;

        public ListingsController(FirestoreDb db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> CreateListing(Listing listing)
        {
            listing.Id = Guid.NewGuid().ToString();

            // 🔥 FORCE UTC
            listing.CreatedAt = DateTime.UtcNow;

            listing.AvailableUntil = DateTime.SpecifyKind(
                listing.AvailableUntil,
                DateTimeKind.Utc
            );

            await _db
                .Collection("listings")
                .Document(listing.Id)
                .SetAsync(listing);

            return Ok(listing);
        }

        [HttpGet]
        public async Task<IActionResult> GetListings()
        {
            var snapshot = await _db.Collection("listings").GetSnapshotAsync();

            var listings = snapshot.Documents
                .Select(doc => doc.ConvertTo<Listing>())
                .ToList();

            return Ok(listings);
        }


    }
}
