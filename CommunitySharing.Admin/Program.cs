using CommunitySharing.Admin.Services;

var builder = WebApplication.CreateBuilder(args);

// Blazor Server
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = true; // show full circuit exceptions (dev only)
});

// HttpClient for API calls
builder.Services.AddScoped(sp => new HttpClient());

// Register your admin API service if present
builder.Services.AddScoped<AdminApiService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
