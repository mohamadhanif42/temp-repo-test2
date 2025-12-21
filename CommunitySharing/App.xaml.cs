using CommunitySharing.Services;
using Microsoft.Maui.Storage;

namespace CommunitySharing
{
    public partial class App : Application
    {
        private readonly AuthManager _authManager;
        private readonly SyncManager _syncManager;

        public App(AuthManager authManager, SyncManager syncManager)
        {
            InitializeComponent();
            _authManager = authManager;
            _syncManager = syncManager;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window();

            window.Page = new ContentPage
            {
                Content = new ActivityIndicator
                {
                    IsRunning = true,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                }
            };

            // Perform login check AFTER window is created
            Task.Run(async () => await InitializeApp(window));

            return window;
        }

        private async Task InitializeApp(Window window)
        {
            SecureStorage.Default.Remove("auth_token");
            Preferences.Remove("IsLoggedIn");

            if (_syncManager != null)
            {
                try
                {
                    // Pull cloud -> local, then push local -> cloud
                    await _syncManager.SyncFromCloudAsync();
                    await _syncManager.SyncToCloudAsync();
                }
                catch (Exception ex)
                {
                    // Log but continue. Use Debug.WriteLine or your logger.
                    System.Diagnostics.Debug.WriteLine("Sync failed: " + ex);
                }
            }

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var appShell = new AppShell();
                window.Page = appShell;

                await appShell.GoToAsync("///LoginPage");
                
            });
        }
        protected override void OnSleep()
        {
            base.OnSleep();
            Task.Run(() =>
            {
                SecureStorage.Default.Remove("auth_token");
                Preferences.Remove("IsLoggedIn");
            });
        }
    }
}

