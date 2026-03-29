using ToDoApplication.Services;
using ToDoApplication.ViewModels;
using ToDoApplication.Pages; // ← ADD THIS LINE

namespace ToDoApplication;

public partial class App : Application
{
    public static TodoViewModel SharedViewModel { get; } = new TodoViewModel();

    public App()
    {
        InitializeComponent();
        StorageService.CurrentUser = null;
        MainPage = new NavigationPage(new SignInPage()); 
    }
}