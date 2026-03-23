using ToDoApplication.Models;
using ToDoApplication.Services;
using System.Threading.Tasks;

namespace ToDoApplication.ViewModels;

public class AuthViewModel
{
    private readonly ApiService _apiService = new ApiService();

    // SIGN IN
    public async Task<bool> SignIn(string email, string password)
    {
        var user = await _apiService.SignInAsync(email, password);
        
        if (user != null)
        {
            StorageService.CurrentUser = user;
            return true;
        }
        return false;
    }

    // SIGN UP (Now perfectly expects 4 arguments)
    public async Task<bool> SignUp(string firstName, string lastName, string email, string password)
    {
        // Send all 4 required pieces to the ApiService
        bool success = await _apiService.SignUpAsync(firstName, lastName, email, password);
        
        if (success)
        {
            return await SignIn(email, password);
        }
        return false;
    }
}