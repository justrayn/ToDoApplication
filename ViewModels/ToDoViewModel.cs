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

    public ICommand CompleteTaskCommand { get; }
    public ICommand DeleteTaskCommand { get; }
    public ICommand RefreshCommand { get; }

    public TodoViewModel()
    {
        _apiService = new ApiService();

        CompleteTaskCommand = new Command<TodoTask>(async (task) => await CompleteTaskAsync(task));
        DeleteTaskCommand = new Command<TodoTask>(async (task) => await DeleteTaskAsync(task));
        RefreshCommand = new Command(async () => await RefreshTasksAsync());

        // FIX: NO auto-load here anymore.
        // Previously the constructor fired a load, then each page's OnAppearing fired another,
        // = 3+ simultaneous requests every tab switch → timeouts + blank lists.
        // Pages now own when to call RefreshTasksAsync().
    }

    private bool _isRefreshing = false;
    private DateTime _lastRefresh = DateTime.MinValue;

    public async Task RefreshTasksAsync(bool force = false)
    {
        // Skip if refreshed less than 3 seconds ago (unless forced)
        if (!force && (DateTime.Now - _lastRefresh).TotalSeconds < 3) return;
        if (_isRefreshing) return;
        _isRefreshing = true;

        try
        {
            if (StorageService.CurrentUser != null && StorageService.CurrentUser.Id > 0)
            {
                int userId = StorageService.CurrentUser.Id;
                var activeTasks = await _apiService.GetTasksAsync(userId, "active");
                var completedTasks = await _apiService.GetTasksAsync(userId, "inactive");

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ToDoTasks.Clear();
                    CompletedTasks.Clear();
                    if (activeTasks != null)
                        foreach (var task in activeTasks) ToDoTasks.Add(task);
                    if (completedTasks != null)
                        foreach (var task in completedTasks) CompletedTasks.Add(task);
                });

                _lastRefresh = DateTime.Now; // record when we last refreshed
            }
        }
        catch (Exception ex) { Console.WriteLine($"Refresh Error: {ex.Message}"); }
        finally { _isRefreshing = false; }
    }
    private async Task CompleteTaskAsync(TodoTask task)
    {
        if (task == null) return;

        bool success = await _apiService.UpdateStatusAsync(task.Id, "inactive");
        if (success)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ToDoTasks.Remove(task);
                task.Status = "inactive";
                CompletedTasks.Add(task);
            });
        }
    }

    private async Task DeleteTaskAsync(TodoTask task)
    {
        if (task == null) return;

        bool success = await _apiService.DeleteTaskAsync(task.Id);
        if (success)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (ToDoTasks.Contains(task)) ToDoTasks.Remove(task);
                if (CompletedTasks.Contains(task)) CompletedTasks.Remove(task);
            });
        }
    }
}