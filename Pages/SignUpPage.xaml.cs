using ToDoApplication.ViewModels;

namespace ToDoApplication.Pages;

public partial class SignUpPage : ContentPage
{
    private AuthViewModel _authViewModel = new AuthViewModel();

    public SignUpPage()
    {
        InitializeComponent();
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            await DisplayAlert("Error", "Passwords do not match!", "OK");
            return;
        }

        var success = _authViewModel.Register(UsernameEntry.Text, EmailEntry.Text, PasswordEntry.Text);
        if (success)
        {
            await DisplayAlert("Success", "Account created!", "OK");
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("Error", "Email already in use.", "OK");
        }
    }

    private async void OnSignInRedirectClicked(object sender, EventArgs e) => await Navigation.PopAsync();
}