using ToDoApplication.Models;
using ToDoApplication.Services;

namespace ToDoApplication.Pages;

[QueryProperty(nameof(Task), "Task")]
public partial class EditPage : ContentPage
{
    private TodoTask _task;

    public TodoTask Task
    {
        get => _task;
        set
        {
            _task = value;
            if (_task != null)
            {
                // We use Dispatcher to ensure the UI is ready before setting text
                Dispatcher.Dispatch(() => {
                    TitleEntry.Text = _task.Title ?? "";
                    DetailsEntry.Text = _task.Details ?? "";
                });
            }
        }
    }

    // CRITICAL: Shell needs this empty constructor!
    public EditPage()
    {
        InitializeComponent();
    }

    private async void OnUpdateClicked(object sender, EventArgs e)
    {
        if (_task == null) return;
        _task.Title = TitleEntry.Text;
        _task.Details = DetailsEntry.Text;
        StorageService.SaveCurrentState();
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCompleteClicked(object sender, EventArgs e)
    {
        if (_task == null) return;
        _task.IsCompleted = true;
        StorageService.SaveCurrentState();
        await Shell.Current.GoToAsync("..");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_task == null) return;
        StorageService.CurrentUser.Tasks.Remove(_task);
        StorageService.SaveCurrentState();
        await Shell.Current.GoToAsync("..");
    }
}