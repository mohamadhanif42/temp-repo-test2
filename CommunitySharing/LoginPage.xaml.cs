using Microsoft.Maui.Storage;
using CommunitySharing.Services;

namespace CommunitySharing;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _api;
    private readonly AuthManager _auth;

    public LoginPage(ApiService api, AuthManager auth)
    {
        InitializeComponent();
        _api = api;
        _auth = auth;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        ValidationMessageLabel.IsVisible = false;

        string email = EmailEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text ?? "";

        if (string.IsNullOrEmpty(email))
        {
            ShowValidationMessage("Please enter your email.");
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            ShowValidationMessage("Please enter your password.");
            return;
        }

        // 1) Firebase sign-in
        var (ok, idToken, err) = await _api.FirebaseSignInAsync(email, password);
        if (!ok || string.IsNullOrEmpty(idToken))
        {
            ValidationMessageLabel.Text = $"Firebase sign-in failed: {err}";
            ValidationMessageLabel.IsVisible = true;
            return;
        }

        // 2) Exchange idToken for app JWT
        var (exOk, appToken, exErr) = await _api.ExchangeFirebaseIdTokenAsync(idToken);
        if (!exOk || string.IsNullOrEmpty(appToken))
        {
            ValidationMessageLabel.Text = $"Login exchange failed: {exErr}";
            ValidationMessageLabel.IsVisible = true;
            return;
        }

        //(for signin checkbox function)

        // 3) Save token only if "Keep me signed in" is checked
        //if (KeepSignedInCheckBox.IsChecked)
        //{
        //    await _auth.SaveTokenAsync(appToken);
        //    Preferences.Set("KeepSignedIn", true);
        //}
        //else
        //{
        //    Preferences.Set("KeepSignedIn", false);
        //}

        // 4) Save email for next time
        Preferences.Set("LastEmail", email);

        await Shell.Current.GoToAsync("//DiscoverPage");

    }

    private void ShowValidationMessage(string message)
    {
        ValidationMessageLabel.Text = message;
        ValidationMessageLabel.IsVisible = true;
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///RegistrationPage");
    }

    private async void OnForgotPasswordClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///ForgotPasswordPage");
    }

    private void OnShowPasswordCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        PasswordEntry.IsPassword = !e.Value;
    }

    private void OnPasswordCompleted(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = true;
        ShowPasswordCheckBox.IsChecked = false;
    }

    private void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.NewTextValue))
            ShowPasswordCheckBox.IsChecked = false;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Prefill email if previously saved
        EmailEntry.Text = Preferences.Get("LastEmail", "");
    }
}
