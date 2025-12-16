namespace CommunitySharing
{
    public partial class DiscoverPage : ContentPage
    {
        public DiscoverPage()
        {
            InitializeComponent();
        }

        // ?? Search button click
        private async void OnSearchClicked(object sender, EventArgs e)
        {
            // FIX: Use 'string?' to allow for a potential null value from SearchEntry.Text?.Trim()
            string? query = SearchEntry.Text?.Trim();

            // Use IsNullOrWhiteSpace to check for null, empty, or only whitespace strings
            if (string.IsNullOrWhiteSpace(query))
            {
                await DisplayAlert("Search", "Please enter something to search.", "OK");
                return;
            }

            // Since we checked, 'query' is now guaranteed to be a non-empty string.
            await DisplayAlert("Search", $"You searched for: {query}", "OK");
        }

        // ?? Bottom Navigation — Discover
        private async void OnNavDiscover(object sender, TappedEventArgs e)
        {
            await DisplayAlert("Discover", "You’re already on the Discover page.", "OK");
        }

        // ?? Bottom Navigation — Upload
        private async void OnNavUpload(object sender, TappedEventArgs e)
        {
            // Note: Assuming 'UploadPage' is correctly registered in your Shell routing
            await Shell.Current.GoToAsync("///UploadPage");
        }

        // ?? Bottom Navigation — Messages
        private async void OnNavMessage(object sender, TappedEventArgs e)
        {
            // Note: Assuming 'MessagesPage' is correctly registered in your Shell routing
            await Shell.Current.GoToAsync("///MessagesPage");
        }

        // ?? Bottom Navigation — Profile
        private async void OnNavProfile(object sender, TappedEventArgs e)
        {
            // Note: Assuming 'ProfilePage' is correctly registered in your Shell routing
            await Shell.Current.GoToAsync("///ProfilePage");
        }
    }
}