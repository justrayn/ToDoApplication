using ToDoApplication.ViewModels;

namespace ToDoApplication.Pages;

public partial class SignInPage : ContentPage
{
    private AuthViewModel _authViewModel = new AuthViewModel();

    public SignInPage()
    {
        InitializeComponent();
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        var success = _authViewModel.Login(EmailEntry.Text, PasswordEntry.Text);
        if (success)
        {
            // Switch from the Login flow to the main Tabbed AppShell
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("Login Failed", "Invalid email or password.", "OK");
        }
    }

    private async void OnSignUpRedirectClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignUpPage());
    }
}