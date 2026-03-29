using ToDoApplication.ViewModels;

namespace ToDoApplication.Pages;

public partial class CompletedPage : ContentPage
{
    public CompletedPage()
    {
        InitializeComponent();

        // FIX: Same shared ViewModel as ToDoPage — so when a task is marked complete
        // on the To Do tab, it instantly appears here without a full reload.
        BindingContext = App.SharedViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Refresh when this tab is opened so the list is always current.
        await App.SharedViewModel.RefreshTasksAsync();
    }

    private async void OnTaskSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Models.TodoTask selectedTask)
        {
            ((ListView)sender).SelectedItem = null;
            await Shell.Current.GoToAsync($"{nameof(EditCompletedPage)}",
                new Dictionary<string, object> { ["Task"] = selectedTask });
        }
    }
}