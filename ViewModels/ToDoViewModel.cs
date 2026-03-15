using System.Collections.ObjectModel;
using ToDoApplication.Models;
using ToDoApplication.Services;

namespace ToDoApplication.ViewModels;

public class TodoViewModel : BindableObject
{
    public ObservableCollection<TodoTask> ToDoTasks { get; set; } = new();
    public ObservableCollection<TodoTask> CompletedTasks { get; set; } = new();

    public void RefreshTasks()
    {
        ToDoTasks.Clear();
        CompletedTasks.Clear();
        if (StorageService.CurrentUser != null)
        {
            foreach (var task in StorageService.CurrentUser.Tasks)
            {
                if (task.IsCompleted) CompletedTasks.Add(task);
                else ToDoTasks.Add(task);
            }
        }
    }
}