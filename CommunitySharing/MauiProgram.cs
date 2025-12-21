using CommunitySharing.Services;
using Microsoft.Extensions.Logging;
using CommunitySharing.Data;

namespace CommunitySharing;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "local.db3");

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                // Material icons (visibility, edit, etc.)
                fonts.AddFont("MaterialIcons-Regular.ttf", "MauiMaterial");
            });

#if DEBUG
var handler = new HttpClientHandler
    {
       ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
builder.Services.AddSingleton(new HttpClient(handler));
#else
builder.Services.AddSingleton(new HttpClient());
#endif

        // Register Pages
        builder.Services.AddSingleton<SplashPage>();
        builder.Services.AddSingleton<IntroductionPage>();
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<RegistrationPage>();
        builder.Services.AddSingleton<ForgotPasswordPage>();
        builder.Services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7080/")
        });
        builder.Services.AddSingleton<AuthManager>();
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<ListingService>();
        builder.Services.AddSingleton<AppDatabase>(sp => new AppDatabase(Path.Combine(FileSystem.AppDataDirectory, "local.db3")));
        builder.Services.AddSingleton<SyncManager>();


        // Main App Pages
        builder.Services.AddSingleton<DiscoverPage>();
        builder.Services.AddSingleton<UploadPage>();
        builder.Services.AddSingleton<MessagesPage>();
        builder.Services.AddSingleton<ProfilePage>();

        return builder.Build();
    }
}
