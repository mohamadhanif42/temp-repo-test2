using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using FirebaseAdmin.Auth;

namespace CommunitySharing.API.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly FirestoreDb _firestore;

        public UsersController(FirestoreDb firestore)
        {
            _firestore = firestore;
        }

        // DTOs
        public class UserListItem
        {
            public string Uid { get; set; } = "";
            public string Email { get; set; } = "";
            public string Role { get; set; } = "User";
            public DateTime? CreatedAt { get; set; }
        }

        public class SetRoleDto
        {
            public string Role { get; set; } = "User";
        }

        // GET /api/admin/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var col = _firestore.Collection("users");
            var snapshot = await col.GetSnapshotAsync();

            var list = new List<UserListItem>(snapshot.Documents.Count);
            foreach (var doc in snapshot.Documents)
            {
                try
                {
                    var uid = doc.Id;
                    var email = doc.TryGetValue<string>("email", out var e) ? e ?? "" : "";
                    var role = doc.TryGetValue<string>("role", out var r) ? r ?? "User" : "User";
                    DateTime? createdAt = null;
                    if (doc.TryGetValue<Timestamp>("createdAt", out var ts))
                        createdAt = ts.ToDateTime();

                    list.Add(new UserListItem
                    {
                        Uid = uid,
                        Email = email,
                        Role = role,
                        CreatedAt = createdAt
                    });
                }
                catch
                {
                    // ignore malformed doc, continue
                }
            }

            return Ok(list);
        }

        // PUT /api/admin/users/{uid}/role
        [HttpPut("{uid}/role")]
        public async Task<IActionResult> SetRole(string uid, [FromBody] SetRoleDto dto)
        {
            if (string.IsNullOrWhiteSpace(uid)) return BadRequest("uid is required");
            if (dto == null || string.IsNullOrWhiteSpace(dto.Role)) return BadRequest("role is required");

            var role = dto.Role.Trim();

            try
            {
                // 1) Set Firebase custom claim
                var claims = new Dictionary<string, object> { { "role", role } };
                await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(uid, claims);

                // 2) Update Firestore user doc
                var docRef = _firestore.Collection("users").Document(uid);
                var update = new Dictionary<string, object>
                {
                    { "role", role },
                    { "updatedAt", Timestamp.GetCurrentTimestamp() }
                };
                await docRef.SetAsync(update, SetOptions.MergeAll);

                return NoContent();
            }
            catch (FirebaseAuthException faex)
            {
                return BadRequest(new { error = faex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // DELETE /api/admin/users/{uid}
        [HttpDelete("{uid}")]
        public async Task<IActionResult> Delete(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid)) return BadRequest("uid is required");

            try
            {
                // 1) Delete from Firebase Auth
                await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);

                // 2) Delete Firestore doc (best-effort)
                var docRef = _firestore.Collection("users").Document(uid);
                await docRef.DeleteAsync();

                return NoContent();
            }
            catch (FirebaseAuthException faex)
            {
                return BadRequest(new { error = faex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
