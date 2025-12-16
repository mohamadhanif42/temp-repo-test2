namespace CommunitySharing
{
    public partial class EditProfilePage : ContentPage
    {
        public EditProfilePage()
        {
            InitializeComponent();
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            // ✅ Go back to Profile tab (absolute route)
            await Shell.Current.GoToAsync("///ProfilePage");
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Profile Updated", "Your changes have been saved successfully.", "OK");

            // ✅ Navigate back to Profile tab (absolute route from root)
            await Shell.Current.GoToAsync("///ProfilePage");
        }

        private async void OnChangePhotoClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Change Photo", "This feature will let users upload a new profile photo.", "OK");
        }
    }
}
