namespace CommunitySharing;

public partial class ForgotPasswordPage : ContentPage
{
    public ForgotPasswordPage()
    {
        InitializeComponent();
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        // TODO: Add logic to send password reset email

        await DisplayAlert("Email Sent", "If an account exists for this email, a reset link has been sent.", "OK");

        await Shell.Current.GoToAsync(".."); // Go back to login page
    }

    private async void OnBackToLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(".."); // Go back one page
    }
}