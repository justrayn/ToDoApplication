using System.Collections.Generic;

namespace ToDoApplication.Models;

public class User
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<TodoTask> Tasks { get; set; } = new List<TodoTask>();
}