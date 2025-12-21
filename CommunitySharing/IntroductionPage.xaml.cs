namespace CommunitySharing; 
public partial class IntroductionPage : ContentPage
{
    public IntroductionPage()
    {
        InitializeComponent();
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(RegistrationPage));
    }
}