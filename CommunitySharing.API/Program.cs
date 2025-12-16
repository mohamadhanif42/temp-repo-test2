using System.Text;
using CommunitySharing.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// Services (controller, swagger, DI registrations)
// --------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Your app services
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<JwtService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// JWT Authentication
var jwtKey = builder.Configuration["JwtKey"] ?? "A9fd!32KlmPq92&$asdlk2039asdLKMZ028asdkQ";
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// --------------------------------------------------
// Firestore (lazy singleton) - created only when resolved
// --------------------------------------------------
// This avoids running Firebase/Firestore code during host build time,
// which previously could trigger unexpected lifecycle errors.
builder.Services.AddSingleton<FirestoreDb>(sp =>
{
    try
    {
        // compute path to JSON (runtime output dir)
        var credentialPath = Path.Combine(AppContext.BaseDirectory, "firebase", "firebase-admin.json");

        if (!File.Exists(credentialPath))
        {
            // helpful message and fail fast at resolve time
            var message = $"FATAL: firebase-admin.json not found at: {credentialPath}";
            Console.WriteLine(message);
            throw new FileNotFoundException(message, credentialPath);
        }

        // load credentials from file
        var googleCredential = GoogleCredential.FromFile(credentialPath);

        // create FirebaseApp only if not already created
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions { Credential = googleCredential });
        }

        // Build FirestoreDb with explicit credentials (no ADC fallback)
        var projectId = builder.Configuration["Firebase:ProjectId"];
        if (string.IsNullOrEmpty(projectId))
        {
            var msg = "Firebase:ProjectId missing in configuration (appsettings.json or env).";
            Console.WriteLine(msg);
            throw new InvalidOperationException(msg);
        }

        var db = new FirestoreDbBuilder
        {
            ProjectId = projectId,
            Credential = googleCredential
        }.Build();

        Console.WriteLine("Firestore initialized successfully.");
        return db;
    }
    catch (Exception ex)
    {
        // rethrow so DI resolution fails with a clear message
        Console.WriteLine("Failed to initialize Firestore: " + ex);
        throw;
    }
});

// --------------------------------------------------
// Build app
// --------------------------------------------------
var app = builder.Build();

// Middleware order — correct
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseRouting();            // must be before auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
