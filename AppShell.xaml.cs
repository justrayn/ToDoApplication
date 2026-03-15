using ToDoApplication;

namespace ToDoApplication;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(Pages.AddTaskPage), typeof(Pages.AddTaskPage));
        Routing.RegisterRoute(nameof(Pages.EditPage), typeof(Pages.EditPage));
        Routing.RegisterRoute(nameof(Pages.EditCompletedPage), typeof(Pages.EditCompletedPage));
    }
}