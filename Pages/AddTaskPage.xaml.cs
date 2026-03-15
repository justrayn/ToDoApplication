using ToDoApplication.Models;
using ToDoApplication.Services;

namespace ToDoApplication.Pages;

public partial class AddTaskPage : ContentPage
{
    public AddTaskPage()
    {
        InitializeComponent();
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleEntry.Text)) 
        {
            await DisplayAlert("Error", "Please enter a title.", "OK");
            return;
        }

        var newTask = new TodoTask { Title = TitleEntry.Text, Details = DetailsEntry.Text };
        StorageService.CurrentUser.Tasks.Add(newTask);
        StorageService.SaveCurrentState();
        
        await MainThread.InvokeOnMainThreadAsync(async () => await Shell.Current.GoToAsync(".."));
    }
}