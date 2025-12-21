using Microsoft.Maui.Controls;
using System;
using Microsoft.Maui.Media;
using CommunitySharing.Services;

namespace CommunitySharing
{
    public partial class UploadPage : ContentPage
    {
        private string photoFilePath;
        private string selectedCategory = "Food"; // Default category

        private readonly ApiService _api;
        public UploadPage(ApiService api)
        {
            InitializeComponent();
            _api = api;
            HighlightTab("Upload");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // FIX: Safely initializes DatePicker properties by checking if the control exists.
            if (this.FindByName<DatePicker>("AvailableUntilDatePicker") is DatePicker datePicker)
            {
                datePicker.MinimumDate = DateTime.Today;
                datePicker.Date = DateTime.Today.AddDays(7);
            }
        }

        private async void OnBackClicked(object sender, EventArgs e) => await Shell.Current.GoToAsync("..");

        // --- 1. CATEGORY SELECTION LOGIC ---
        private void OnCategoryTapped(object sender, TappedEventArgs e)
        {
            if (sender is Border tappedBorder)
            {
                // 1. Update the selected category variable using the ClassId ("Food", "Books", etc.)
                selectedCategory = tappedBorder.ClassId;

                // 2. Reset all categories to the "Inactive" style
                SetCategoryStyle(CatFood, LblFood, false);
                SetCategoryStyle(CatBooks, LblBooks, false);
                SetCategoryStyle(CatClothes, LblClothes, false);

                // 3. Set the tapped category to the "Active" style
                if (tappedBorder == CatFood) SetCategoryStyle(CatFood, LblFood, true);
                else if (tappedBorder == CatBooks) SetCategoryStyle(CatBooks, LblBooks, true);
                else if (tappedBorder == CatClothes) SetCategoryStyle(CatClothes, LblClothes, true);
            }
        }

        private void SetCategoryStyle(Border border, Label label, bool isActive)
        {
            if (isActive)
            {
                border.Stroke = Color.FromArgb("#34C759");       // Green Border
                border.BackgroundColor = Color.FromArgb("#E1F5E5"); // Light Green Background
                label.TextColor = Color.FromArgb("#34C759");        // Green Text
                label.FontAttributes = FontAttributes.Bold;
            }
            else
            {
                border.Stroke = Color.FromArgb("#E5E7EB");       // Gray Border
                border.BackgroundColor = Colors.White;              // White Background
                label.TextColor = Color.FromArgb("#6B7280");        // Gray Text
                label.FontAttributes = FontAttributes.None;
            }
        }

        // --- 2. PHOTO LOGIC ---

        private async void OnPhotoUploadTapped(object sender, TappedEventArgs e)
        {
            string action = await DisplayActionSheet("Add Listing Photo", "Cancel", null, "Take Photo", "Pick from Gallery");
            if (action == "Take Photo") await TakePhotoAsync();
            else if (action == "Pick from Gallery") await PickPhotoAsync();
        }

        private void OnRemovePhotoClicked(object sender, EventArgs e)
        {
            photoFilePath = null;
            PhotoPreviewImage.Source = null;

            // Toggle UI State back to "Empty"
            PhotoPlaceholder.IsVisible = true;
            PhotoFrame.IsVisible = false;
            DeletePhotoButton.IsVisible = false;
        }

        private async Task LoadPhotoAsync(FileResult photo)
        {
            photoFilePath = photo.FullPath;
            var stream = await photo.OpenReadAsync();
            PhotoPreviewImage.Source = ImageSource.FromStream(() => stream);

            // Toggle UI State to "Filled"
            PhotoPlaceholder.IsVisible = false;
            PhotoFrame.IsVisible = true;
            DeletePhotoButton.IsVisible = true;
        }

        private async Task TakePhotoAsync()
        {
            if (!MediaPicker.Default.IsCaptureSupported) { await DisplayAlert("Error", "Camera not supported", "OK"); return; }
            try
            {
                FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null) await LoadPhotoAsync(photo);
            }
            catch (Exception ex) { await DisplayAlert("Error", $"Camera error: {ex.Message}", "OK"); }
        }

        private async Task PickPhotoAsync()
        {
            try
            {
                FileResult photo = await MediaPicker.Default.PickPhotoAsync();
                if (photo != null) await LoadPhotoAsync(photo);
            }
            catch (Exception ex) { await DisplayAlert("Error", $"Gallery error: {ex.Message}", "OK"); }
        }

        // --- 3. FORM SUBMISSION ---

        private async void OnUploadClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(photoFilePath))
            {
                await DisplayAlert("Photo Required", "Please add a photo.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(TitleEntry.Text))
            {
                await DisplayAlert("Title Required", "Please add a title.", "OK");
                return;
            }

            var imageBase64 = await GetPhotoBase64Async();
            if (imageBase64 == null)
            {
                await DisplayAlert("Error", "Could not read image.", "OK");
                return;
            }

            string ownerEmail = Preferences.Get("LastEmail", "");

            (bool success, string? errorMessage) = await _api.CreateListingAsync(
            title: TitleEntry.Text,
            category: selectedCategory,
            availableUntil: AvailableUntilDatePicker.Date,
            imageBase64: imageBase64,
            ownerEmail: ownerEmail
);

            if (!success)
            {
                await DisplayAlert("Upload Failed", errorMessage ?? "Unknown error", "OK");
                return;
            }

            await DisplayAlert("Success", "Listing published!", "OK");
            await Shell.Current.GoToAsync("..");
        }


        // --- 4. NAVIGATION LOGIC ---

        private async void OnNavDiscover(object sender, EventArgs e) => await Shell.Current.GoToAsync("///DiscoverPage");
        private async void OnNavUpload(object sender, EventArgs e) => await Shell.Current.GoToAsync("///UploadPage");
        private async void OnNavMessage(object sender, EventArgs e) => await Shell.Current.GoToAsync("///MessagesPage");
        private async void OnNavProfile(object sender, EventArgs e) => await Shell.Current.GoToAsync("///ProfilePage");

        private void HighlightTab(string selected)
        {
            ResetTabColors(DiscoverTab); ResetTabColors(UploadTab); ResetTabColors(MessageTab); ResetTabColors(ProfileTab);
            switch (selected) { case "Upload": SetActiveTab(UploadTab); break; }
        }

        private void ResetTabColors(VerticalStackLayout tab)
        {
            if (tab.Children.Count > 1 && tab.Children[0] is Image icon && tab.Children[1] is Label label)
            {
                icon.Opacity = 0.4;
                label.TextColor = Color.FromArgb("#9CA3AF");
            }
        }

        private void SetActiveTab(VerticalStackLayout tab)
        {
            if (tab.Children.Count > 1 && tab.Children[0] is Image icon && tab.Children[1] is Label label)
            {
                icon.Opacity = 1.0;
                label.TextColor = Color.FromArgb("#34C759");
            }
        }

        private async Task<string?> GetPhotoBase64Async()
        {
            if (string.IsNullOrEmpty(photoFilePath))
                return null;

            var bytes = await File.ReadAllBytesAsync(photoFilePath);
            return Convert.ToBase64String(bytes);
        }

    }
}