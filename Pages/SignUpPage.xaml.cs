using System;
using System.Threading.Tasks;
using ToDoApplication.ViewModels;
using ToDoApplication.Services;

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
        try
        {
            string firstName = FirstNameEntry.Text ?? "";
            string lastName = LastNameEntry.Text ?? "";
            string email = EmailEntry.Text ?? "";
            string password = PasswordEntry.Text ?? "";
            string confirmPassword = ConfirmPasswordEntry.Text ?? "";

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

            bool success = await _viewModel.SignUp(firstName, lastName, email, password);

            if (success)
            {
                await DisplayAlert("Success!", "Account created. Please sign in.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Registration failed. The email might already exist.", "OK");
            }
        }
        catch (Exception ex)
        {
            // This will show us exactly what's crashing
            await DisplayAlert("CRASH DETAILS", ex.GetType().Name + ": " + ex.Message + "\n\n" + ex.StackTrace, "OK");
        }
    }

    private async void OnSignInRedirectClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}