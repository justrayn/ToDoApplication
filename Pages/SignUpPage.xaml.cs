using System;
using System.Threading.Tasks;
using ToDoApplication.ViewModels;

namespace ToDoApplication.Pages;

public partial class SignUpPage : ContentPage
{
    private readonly AuthViewModel _viewModel = new AuthViewModel();

    public SignUpPage()
    {
        InitializeComponent();
    }

    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        // 1. Grab text from the 4 boxes on your screen
        string firstName = FirstNameEntry.Text ?? ""; 
        string lastName = LastNameEntry.Text ?? ""; 
        string email = EmailEntry.Text ?? "";
        string password = PasswordEntry.Text ?? "";
        string confirmPassword = ConfirmPasswordEntry.Text ?? "";

        // --- THE BOUNCER ---
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || 
            string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Wait!", "Please fill in all the boxes.", "OK");
            return; 
        }

        if (!email.Contains("@") || !email.Contains("."))
        {
            await DisplayAlert("Invalid Email", "Please enter a valid email address.", "OK");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("Oops!", "Your passwords do not match.", "OK");
            return;
        }

        // 2. Pass exactly 4 items to the ViewModel
        bool success = await _viewModel.SignUp(firstName, lastName, email, password);

        if (success)
        {
            if (Application.Current != null)
                Application.Current.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("Error", "Registration failed. The email might already exist.", "OK");
        }
    }

    private async void OnSignInRedirectClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}