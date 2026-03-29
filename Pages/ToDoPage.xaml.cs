using ToDoApplication.ViewModels;

namespace ToDoApplication.Pages;

public partial class ToDoPage : ContentPage
{
    public ToDoPage()
    {
        InitializeComponent();

        // FIX: Use the single shared ViewModel from App so ToDoPage and CompletedPage
        // always read from and write to the SAME list.
        // Previously each page's XAML created its own TodoViewModel instance — so marking
        // a task complete on one page was invisible to the other page's list.
        BindingContext = App.SharedViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Refresh only when this tab becomes visible — not on construction.
        await App.SharedViewModel.RefreshTasksAsync();
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
                ((ListView)sender).SelectedItem = null;
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