using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using CommunitySharing.API.Services;

namespace CommunitySharing.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwt;
        private readonly FirestoreDb? _firestore;

        public AuthController(JwtService jwt, IServiceProvider services)
        {
            _jwt = jwt;
            _firestore = services.GetService(typeof(FirestoreDb)) as FirestoreDb;
        }

        public class RegisterDto { public string Email { get; set; } = ""; public string Password { get; set; } = ""; }
        public class LoginIdTokenDto { public string IdToken { get; set; } = ""; }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email and password required.");

            try
            {
                var args = new UserRecordArgs { Email = dto.Email, Password = dto.Password, EmailVerified = false };
                var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);

                if (_firestore != null)
                {
                    var doc = _firestore.Collection("users").Document(userRecord.Uid);
                    var docData = new Dictionary<string, object>
                    {
                        { "email", dto.Email },
                        { "role", "User" },
                        { "createdAt", Timestamp.GetCurrentTimestamp() }
                    };
                    await doc.SetAsync(docData);
                }

                return Ok(new { message = "Registered", uid = userRecord.Uid });
            }
            catch (FirebaseAuthException fex)
            {
                return BadRequest(new { error = fex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginIdTokenDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.IdToken))
                return BadRequest("idToken required.");

            try
            {
                FirebaseToken decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(dto.IdToken);
                string uid = decoded.Uid;
                string email = decoded.Claims.TryGetValue("email", out var e) ? e?.ToString() ?? "" : "";

                // Try read role from custom claims first
                string role = "User";
                if (decoded.Claims != null && decoded.Claims.TryGetValue("role", out var cRole))
                {
                    role = cRole?.ToString() ?? "User";
                }
                else if (_firestore != null)
                {
                    var snapshot = await _firestore.Collection("users").Document(uid).GetSnapshotAsync();
                    if (snapshot.Exists && snapshot.TryGetValue<string>("role", out var fsRole))
                        role = fsRole ?? "User";
                }

                // Generate application JWT (signed by JwtService)
                var token = _jwt.GenerateTokenFromFirebase(uid, email, role);

                return Ok(new { token, role });
            }
            catch (FirebaseAuthException ex)
            {
                return Unauthorized(new { error = "Invalid firebase token", details = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
