using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ToDoApplication.Models;
using ToDoApplication.Services;

namespace ToDoApplication.ViewModels;

public class TodoViewModel : BindableObject
{
    private readonly ApiService _apiService;

    public ObservableCollection<TodoTask> ToDoTasks { get; set; } = new();
    public ObservableCollection<TodoTask> CompletedTasks { get; set; } = new();

    // These commands let the UI talk to our methods
    public ICommand CompleteTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand RefreshCommand { get; }
    public TodoViewModel()
    {
        _apiService = new ApiService();

        // Initialize the commands
        CompleteTaskCommand = new Command<TodoTask>(async (task) => await CompleteTaskAsync(task));
        DeleteTaskCommand = new Command<TodoTask>(async (task) => await DeleteTaskAsync(task));
        RefreshCommand = new Command(async () => await RefreshTasksAsync());
    }
    private bool _isRefreshing = false;

    public async Task RefreshTasksAsync()
    {
        // 1. Lock to prevent multiple refreshes
        if (_isRefreshing) return;
        _isRefreshing = true;

        try
        {
            if (StorageService.CurrentUser != null && StorageService.CurrentUser.Id > 0)
            {
                int userId = StorageService.CurrentUser.Id;

                // 2. Fetch from the internet (happens on a background thread so the app doesn't freeze)
                var activeTasks = await _apiService.GetTasksAsync(userId, "active");
                var completedTasks = await _apiService.GetTasksAsync(userId, "inactive");

                // 3. FORCE the app to switch back to the Main UI Thread before touching the screen
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ToDoTasks.Clear();
                    CompletedTasks.Clear();

                    if (activeTasks != null)
                    {
                        foreach (var task in activeTasks) ToDoTasks.Add(task);
                    }

                    if (completedTasks != null)
                    {
                        foreach (var task in completedTasks) CompletedTasks.Add(task);
                    }
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Refresh UI Error: {ex.Message}");
        }
        finally
        {
            // Always unlock when finished
            _isRefreshing = false; 
        }
    }

    // METHOD TO COMPLETE A TASK
    private async Task CompleteTaskAsync(TodoTask task)
    {
        if (task == null) return;

        // 1. Tell the server to change status to "inactive" (Completed)
        bool success = await _apiService.UpdateStatusAsync(task.Id, "inactive");

        if (success)
        {
            // 2. Move it visually in the app without needing to reload everything
            ToDoTasks.Remove(task);
            task.Status = "inactive";
            CompletedTasks.Add(task);
        }
    }

    // METHOD TO DELETE A TASK
    private async Task DeleteTaskAsync(TodoTask task)
    {
        if (task == null) return;

        // 1. Tell the server to permanently delete the task
        bool success = await _apiService.DeleteTaskAsync(task.Id);

        if (success)
        {
            // 2. Remove it visually from whatever list it was in
            if (ToDoTasks.Contains(task)) ToDoTasks.Remove(task);
            if (CompletedTasks.Contains(task)) CompletedTasks.Remove(task);
        }
    }
}