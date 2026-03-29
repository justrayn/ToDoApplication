using ToDoApplication.Models;
using ToDoApplication.Services;

namespace ToDoApplication.Pages;

public partial class AddTaskPage : ContentPage
{
    private readonly ApiService _apiService;

    public AddTaskPage()
    {
        InitializeComponent();
        _apiService = new ApiService(); // Initialize the API service
    }
// 1. Create the lock
    private bool _isSaving = false;

    private async void OnAddClicked(object sender, EventArgs e)
    {
        if (_isSaving) return; 

        if (string.IsNullOrWhiteSpace(TitleEntry.Text)) 
        {
            await DisplayAlert("Error", "Please enter a title.", "OK");
            return;
        }

        // Lock it
        _isSaving = true; 
        
        // --- NEW: Change button text so you know it's thinking ---
        var button = (Button)sender;
        button.Text = "Saving..."; 

        int userId = StorageService.CurrentUser.Id;
        string title = TitleEntry.Text;
        string details = DetailsEntry.Text ?? string.Empty;

        bool isSuccess = await _apiService.AddTaskAsync(title, details, userId);

        if (isSuccess)
        {
            
            App.SharedViewModel.ToDoTasks.Add(new TodoTask
            {
                Title = title,
                Details = details,
                Status = "active"
            });
            await MainThread.InvokeOnMainThreadAsync(async () => await Shell.Current.GoToAsync(".."));
        }
        else
        {
            await DisplayAlert("Error", "Failed to save the task to the server.", "OK");
            
            // --- NEW: Change text back and unlock if it fails ---
            button.Text = "Add Task"; 
            _isSaving = false; 
        }
    }
}