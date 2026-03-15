using ToDoApplication.Services;

namespace ToDoApplication.Pages;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (StorageService.CurrentUser != null)
            UsernameLabel.Text = StorageService.CurrentUser.Username;
    }

    private void OnSignOutClicked(object sender, EventArgs e)
    {
        StorageService.CurrentUser = null;
        // Kick them back to the Sign In page
        Application.Current.MainPage = new NavigationPage(new SignInPage());
    }
}