using ToDoApplication.Models;
using ToDoApplication.Services;
using System.Threading.Tasks;

namespace ToDoApplication.ViewModels;

public class AuthViewModel
{
    private readonly ApiService _apiService = new ApiService();

    public async Task<bool> SignIn(string email, string password)
    {
        try
        {
            var user = await _apiService.SignInAsync(email, password);
            if (user != null)
            {
                StorageService.CurrentUser = user;
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SignIn Error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SignUp(string firstName, string lastName, string email, string password)
    {
        try
        {
            bool registered = await _apiService.SignUpAsync(firstName, lastName, email, password);
            return registered;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SignUp Error: {ex.Message}");
            return false;
        }
    }
}