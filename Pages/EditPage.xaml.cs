using ToDoApplication.Models;
using ToDoApplication.Services;

namespace ToDoApplication.Pages;

[QueryProperty(nameof(Task), "Task")]
public partial class EditPage : ContentPage
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

    public EditPage()
    {
        InitializeComponent();
        _apiService = new ApiService(); // Initialize it
    }

    private async void OnUpdateClicked(object sender, EventArgs e)
    {
        if (_task == null) return;
        
        // 1. Send the new text to the server
        bool success = await _apiService.EditTaskAsync(_task.Id, TitleEntry.Text, DetailsEntry.Text ?? "");
        
        if (success) await Shell.Current.GoToAsync("..");
        else await DisplayAlert("Error", "Failed to update task on server.", "OK");
    }

    private async void OnCompleteClicked(object sender, EventArgs e)
    {
        if (_task == null) return;
        
        // 1. Tell server status is now inactive
        bool success = await _apiService.UpdateStatusAsync(_task.Id, "inactive");
        
        if (success) await Shell.Current.GoToAsync("..");
        else await DisplayAlert("Error", "Failed to complete task.", "OK");
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_task == null) return;
        
        // 1. Tell server to delete it entirely
        bool success = await _apiService.DeleteTaskAsync(_task.Id);
        
        if (success) await Shell.Current.GoToAsync("..");
        else await DisplayAlert("Error", "Failed to delete task.", "OK");
    }
}