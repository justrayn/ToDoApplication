using ToDoApplication.Models;
using ToDoApplication.Services;

namespace ToDoApplication.ViewModels;

public class AuthViewModel
{
    public bool Login(string email, string password)
    {
        var users = StorageService.LoadAllUsers();
        var user = users.FirstOrDefault(u => u.Email == email && u.Password == password);
        
        if (user != null)
        {
            StorageService.CurrentUser = user;
            return true;
        }
        return false;
    }

    public bool Register(string username, string email, string password)
    {
        var users = StorageService.LoadAllUsers();
        if (users.Any(u => u.Email == email)) return false; // Email already exists

        var newUser = new User { Username = username, Email = email, Password = password };
        StorageService.CurrentUser = newUser;
        StorageService.SaveCurrentState();
        return true;
    }
}