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
        // FIX 1: Null guard — if the user left a field empty, EmailEntry.Text is null,
        // which crashes Uri.EscapeDataString() with "Value cannot be null"
        string email = EmailEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text ?? "";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Error", "Please enter your email and password.", "OK");
            return;
        }

        bool isValid = await _viewModel.SignIn(email, password);

        if (isValid)
        {
            // FIX 2: Refresh tasks BEFORE navigating so the list is ready when the
            // dashboard appears — no more blank screen on first login.
            await App.SharedViewModel.RefreshTasksAsync();

            // Replace the whole page stack with AppShell (tabs)
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("Error", "Invalid email or password.", "OK");
        }
    }

    private async void OnSignUpRedirectClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SignUpPage());
    }
}