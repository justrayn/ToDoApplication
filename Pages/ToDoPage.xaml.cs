using ToDoApplication.ViewModels;

namespace ToDoApplication.Pages;

public partial class ToDoPage : ContentPage
{
    public ToDoPage()
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

    private async void OnAddTaskClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddTaskPage));
    }

    private async void OnTaskSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Models.TodoTask selectedTask)
        {
            try 
            {
                // Deselect immediately to prevent double-clicks
                ((ListView)sender).SelectedItem = null;

                // Use the nameof to ensure no typos
                await Shell.Current.GoToAsync(nameof(Pages.EditPage), new Dictionary<string, object>
                {
                    { "Task", selectedTask }
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Navigation Error", ex.Message, "OK");
            }
        }
    }
}