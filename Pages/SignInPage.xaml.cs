using ToDoApplication.ViewModels;

namespace ToDoApplication.Pages;

public partial class SignInPage : ContentPage
{
    private AuthViewModel _viewModel = new AuthViewModel();

    public SignInPage()
    {
        InitializeComponent();
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;

        // We changed "Login" to "SignIn" and added "await"
        bool isValid = await _viewModel.SignIn(email, password);

        if (isValid)
        {
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("Error", "Invalid email or password", "OK");
        }
    }

    private async void OnSignUpRedirectClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignUpPage());
    }
}