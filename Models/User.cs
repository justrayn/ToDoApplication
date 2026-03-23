using System.Collections.Generic;

namespace ToDoApplication.Models;

public class User
{
    // The magical ID from the API!
    public int Id { get; set; } 
    
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<TodoTask> Tasks { get; set; } = new List<TodoTask>();
}