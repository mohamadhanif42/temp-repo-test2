using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using FirebaseAdmin.Auth;

namespace CommunitySharing.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")] // only existing admins can call this
    public class AdminController : ControllerBase
    {
        private readonly FirestoreDb _firestore;

        public AdminController(FirestoreDb firestore)
        {
            _firestore = firestore;
        }

        // Set a user as admin by Firebase UID
        [HttpPost("set-admin/{uid}")]
        public async Task<IActionResult> SetAdminByUid(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid))
                return BadRequest("uid required.");

            // 1) set custom claim on Firebase user
            var claims = new Dictionary<string, object> { { "role", "Admin" } };
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, claims);

            // 2) update Firestore user doc (optional but recommended)
            var docRef = _firestore.Collection("users").Document(uid);
            await docRef.SetAsync(new { role = "Admin" }, SetOptions.MergeAll);

            return Ok(new { message = "User promoted to Admin", uid });
        }

        // Convenience: set admin by email (look up uid)
        [HttpPost("set-admin-by-email")]
        public async Task<IActionResult> SetAdminByEmail([FromBody] EmailDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("email required.");

            try
            {
                var userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(dto.Email);
                var uid = userRecord.Uid;

                // reuse method above:
                var claims = new Dictionary<string, object> { { "role", "Admin" } };
                await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, claims);

                var docRef = _firestore.Collection("users").Document(uid);
                await docRef.SetAsync(new { role = "Admin" }, SetOptions.MergeAll);

                return Ok(new { message = "User promoted to Admin", email = dto.Email, uid });
            }
            catch (FirebaseAuthException fex)
            {
                return NotFound(new { error = fex.Message });
            }
        }

        public class EmailDto { public string Email { get; set; } = ""; }
    }
}
