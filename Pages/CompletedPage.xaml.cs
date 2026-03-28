using ToDoApplication.ViewModels;

namespace ToDoApplication.Pages;

public partial class CompletedPage : ContentPage
{
    public CompletedPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is TodoViewModel viewModel)
        {
            // Changed to use the new Async method we built!
            await viewModel.RefreshTasksAsync();
        }
    }

    private async void OnTaskSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Models.TodoTask selectedTask)
        {
            ((ListView)sender).SelectedItem = null;
            // Redirect to the special "Edit Completed" page
            await Shell.Current.GoToAsync($"{nameof(EditCompletedPage)}", 
                new Dictionary<string, object> { ["Task"] = selectedTask });

        }
    }
}