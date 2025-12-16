using Microsoft.Maui.Controls;
using System;

namespace CommunitySharing
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            HighlightTab("Profile");
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnEditProfileClicked(object sender, EventArgs e)
        {
    
            await Shell.Current.GoToAsync(nameof(EditProfilePage));
        }
        private async void OnNavDiscover(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///DiscoverPage");
        }

        private async void OnNavUpload(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///UploadPage");
        }

        private async void OnNavMessage(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///MessagesPage");
        }

        private async void OnNavProfile(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///ProfilePage");
        }

        private void HighlightTab(string selected)
        {
            ResetTabColors(DiscoverTab);
            ResetTabColors(UploadTab);
            ResetTabColors(MessageTab);
            ResetTabColors(ProfileTab);

            switch (selected)
            {
                case "Discover": SetActiveTab(DiscoverTab); break;
                case "Upload": SetActiveTab(UploadTab); break;
                case "Message": SetActiveTab(MessageTab); break;
                case "Profile": SetActiveTab(ProfileTab); break;
            }
        }

        private void ResetTabColors(VerticalStackLayout tab)
        {
            if (tab.Children[0] is Image icon)
                icon.Opacity = 0.6;
            if (tab.Children[1] is Label label)
                label.TextColor = Colors.Black;
        }

        private void SetActiveTab(VerticalStackLayout tab)
        {
            if (tab.Children[0] is Image icon)
                icon.Opacity = 1.0;
            if (tab.Children[1] is Label label)
                label.TextColor = Color.FromArgb("#34C759");
        }
    }
}
