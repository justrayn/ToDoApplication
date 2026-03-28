using ToDoApplication.Models;
using ToDoApplication.Services;

namespace ToDoApplication.Pages;

[QueryProperty(nameof(Task), "Task")]
public partial class EditCompletedPage : ContentPage
{
    private TodoTask _task;
    private readonly ApiService _apiService; // Added ApiService

    public TodoTask Task
    {
        get => _task;
        set
        {
            _task = value;
            if (_task != null)
            {
                Dispatcher.Dispatch(() => {
                    TitleEntry.Text = _task.Title ?? "";
                    DetailsEntry.Text = _task.Details ?? "";
                });
            }
        }
    }

    public EditCompletedPage()
    {
        InitializeComponent();
        _apiService = new ApiService(); // Initialize it
    }

    private async void OnUpdateClicked(object sender, EventArgs e)
    {
        if (_task == null) return;

        // 1. Update text on the server
        bool success = await _apiService.EditTaskAsync(_task.Id, TitleEntry.Text, DetailsEntry.Text ?? "");
        
        if (success) await Shell.Current.GoToAsync("..");
        else await DisplayAlert("Error", "Failed to update task on server.", "OK");
    }

    private async void OnIncompleteClicked(object sender, EventArgs e)
    {
        if (_task == null) return;

        // 1. Tell server to change status back to active
        bool success = await _apiService.UpdateStatusAsync(_task.Id, "active"); 
        
        if (success) await Shell.Current.GoToAsync("..");
        else await DisplayAlert("Error", "Failed to reactivate task.", "OK");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_task == null) return;

        // 1. Tell server to delete it permanently
        bool success = await _apiService.DeleteTaskAsync(_task.Id);
        
        if (success) await Shell.Current.GoToAsync("..");
        else await DisplayAlert("Error", "Failed to delete task.", "OK");
    }
}