using ToDoApplication.Models;
using ToDoApplication.Services;

namespace ToDoApplication.Pages;

[QueryProperty(nameof(Task), "Task")]
public partial class EditCompletedPage : ContentPage
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
                // Ensure UI elements are ready before setting text
                Dispatcher.Dispatch(() => {
                    TitleEntry.Text = _task.Title ?? "";
                    DetailsEntry.Text = _task.Details ?? "";
                });
            }
        }
    }

    // REQUIRED: Parameterless constructor for Shell Navigation
    public EditCompletedPage()
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

    private async void OnIncompleteClicked(object sender, EventArgs e)
    {
        if (_task == null) return;

        _task.IsCompleted = false; // Move back to To Do
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