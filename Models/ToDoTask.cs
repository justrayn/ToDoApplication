namespace ToDoApplication.Models;

public class TodoTask
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; }
    public string Details { get; set; }
    public bool IsCompleted { get; set; }
}