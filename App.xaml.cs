namespace ToDoApplication;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Start directly with the SignInPage. 
        // No NavigationPage wrapper needed.
        // MainPage = new Pages.SignInPage();
        MainPage = new NavigationPage(new Pages.SignInPage());
    }
}