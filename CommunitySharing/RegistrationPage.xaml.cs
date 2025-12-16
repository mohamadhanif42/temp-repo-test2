using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Maui.Storage;
using CommunitySharing.Services;

namespace CommunitySharing;

public partial class RegistrationPage : ContentPage
{
    private class RegisterResponse
    {
        public string Token { get; set; } = string.Empty;
    }

    private readonly HttpClient _httpClient;

    public RegistrationPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient(); // Ideally inject via DI
    }

    // SIGN UP BUTTON CLICK HANDLER
    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        // Hide old message
        ValidationMessageLabel.IsVisible = false;

        // Read user input
        string? fullName = FullNameEntry.Text?.Trim();
        string? email = EmailEntry.Text?.Trim();
        string password = PasswordEntry.Text;
        string confirmPassword = ConfirmPasswordEntry.Text;
        bool termsAccepted = TermsCheckBox.IsChecked;

        // --- VALIDATION ---
        if (string.IsNullOrEmpty(fullName))
        {
            ShowValidationMessage("Please enter your full name.");
            return;
        }

        if (string.IsNullOrEmpty(email))
        {
            ShowValidationMessage("Please enter your email address.");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowValidationMessage("Please enter a password.");
            return;
        }

        if (string.IsNullOrEmpty(confirmPassword))
        {
            ShowValidationMessage("Please confirm your password.");
            return;
        }

        if (password != confirmPassword)
        {
            ShowValidationMessage("Passwords do not match.");
            return;
        }

        if (!termsAccepted)
        {
            ShowValidationMessage("Please agree to the Terms & Conditions before signing up.");
            return;
        }

        // --- SEND DATA TO BACKEND ---
        var registerData = new
        {
            FullName = fullName,
            Email = email,
            Password = password
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{ApiConfig.BaseUrl}/api/auth/register",
                registerData
            );

            if (response.IsSuccessStatusCode)
            {
                // Optional: read token if returned
                var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();

                // Save email for convenience
                Preferences.Set("LastEmail", email);

                // Navigate to DiscoverPage regardless of token
                await Shell.Current.GoToAsync("///DiscoverPage");
            }
            else
            {
                // Read error message from backend if available
                var error = await response.Content.ReadAsStringAsync();
                ShowValidationMessage($"Registration failed: {error}");
            }
        }
        catch (Exception ex)
        {
            ShowValidationMessage($"Error connecting to server: {ex.Message}");
        }
    }

    // Show validation message under the Sign-Up button
    private void ShowValidationMessage(string message)
    {
        ValidationMessageLabel.Text = message;
        ValidationMessageLabel.IsVisible = true;
    }

    // LOGIN CLICK HANDLER
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///LoginPage");
    }

    // SHOW PASSWORD CHECKBOX
    private void OnShowPasswordCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        PasswordEntry.IsPassword = !e.Value;
        ConfirmPasswordEntry.IsPassword = !e.Value;
    }

    // HIDE PASSWORD WHEN USER PRESSES NEXT
    private void OnPasswordCompleted(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = true;
        ConfirmPasswordEntry.IsPassword = true;
        ShowPasswordCheckBox.IsChecked = false;
    }

    // RESET SHOW PASSWORD CHECKBOX IF PASSWORD CLEARED
    private void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.NewTextValue))
            ShowPasswordCheckBox.IsChecked = false;
    }
}
