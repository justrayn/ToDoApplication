using System.Net.Http.Json;
using ToDoApplication.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ToDoApplication.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://todo-list.dcism.org";

    public ApiService()
    {
        // 1. Create a handler that ignores SSL certificate errors
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
        {
            return true; // Force the phone to trust the school's server
        };

        // 2. Pass the handler into your HttpClient
        _httpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(60)
        };
    }
    // STEP A: SIGN IN
    public async Task<User> SignInAsync(string email, string password)
    {
        try
        {
            // FIX 1: URL-encode email and password so special characters
            // (like @, +, #, spaces) don't break the request on any device
            var encodedEmail = Uri.EscapeDataString(email);
            var encodedPassword = Uri.EscapeDataString(password);

            var url = $"{BaseUrl}/signin_action.php?email={encodedEmail}&password={encodedPassword}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<UserData>>(json);

                if (apiResponse?.Status == 200 && apiResponse.Data != null)
                {
                    return new User
                    {
                        Id = apiResponse.Data.Id,
                        Email = apiResponse.Data.Email,
                        Username = $"{apiResponse.Data.Fname} {apiResponse.Data.Lname}"
                    };
                }
                else
                {
                    // Log the actual error message from the server for easier debugging
                    Console.WriteLine($"Sign In Failed: {apiResponse?.Message}");
                }
            }
        }
        catch (TaskCanceledException)
        {
            // FIX 4: Friendly timeout message instead of a silent hang
            Console.WriteLine("Sign In Error: Request timed out. Check your internet connection.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Sign In Error: {ex.Message}");
        }
        return null;
    }

    // STEP B: SIGN UP
    public async Task<bool> SignUpAsync(string firstName, string lastName, string email, string password)
    {
        try
        {
            Console.WriteLine($"Signing up: first={firstName}, last={lastName}, email={email}");

            // FIX: Send as JSON body instead of form data
            // The server reads php://input (JSON), not $_POST (form)
            var payload = new
            {
                first_name = firstName,
                last_name = lastName,
                email = email,
                password = password,
                confirm_password = password
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(payload),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{BaseUrl}/signup_action.php", content);
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"SignUp Server Response: {json}");

            // Strip any PHP warnings before parsing JSON
            var jsonStart = json.IndexOf('{');
            if (jsonStart > 0) json = json.Substring(jsonStart);

            var result = JsonConvert.DeserializeObject<ApiResponse<object>>(json);
            return result?.Status == 200;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Sign Up Error: {ex.Message}");
            return false;
        }
    }

    // FETCH TASKS FROM SERVER
    public async Task<List<TodoTask>> GetTasksAsync(int userId, string status)
    {
        try
        {
            var url = $"{BaseUrl}/getItems_action.php?status={status}&user_id={userId}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"--- GET TASKS JSON ({status}): {json} ---");

                var result = JsonConvert.DeserializeObject<ApiResponse<JToken>>(json);

                if (result?.Status == 200 && result.Data != null)
                {
                    if (result.Data.Type == JTokenType.Array)
                    {
                        return result.Data.ToObject<List<TodoTask>>() ?? new List<TodoTask>();
                    }
                    else if (result.Data.Type == JTokenType.Object)
                    {
                        var dict = result.Data.ToObject<Dictionary<string, TodoTask>>();
                        return dict?.Values.ToList() ?? new List<TodoTask>();
                    }
                }
            }
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine($"Get Tasks Error: Request timed out.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get Tasks Error: {ex.Message}");
        }
        return new List<TodoTask>();
    }

    // ADD A TASK TO THE SERVER
    public async Task<bool> AddTaskAsync(string name, string description, int userId)
    {
        try
        {
            var payload = new
            {
                item_name = name,
                item_description = description,
                user_id = userId
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(payload),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{BaseUrl}/addItem_action.php", content);
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"--- ADD TASK SERVER RESPONSE: {json} ---");

            var result = JsonConvert.DeserializeObject<ApiResponse<object>>(json);
            return result?.Status == 200;
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Add Task Error: Request timed out.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Add Task Error: {ex.Message}");
        }
        return false;
    }

    // UPDATE TASK STATUS ON SERVER (active OR inactive)
    public async Task<bool> UpdateStatusAsync(int itemId, string status)
    {
        try
        {
            var payload = new { status = status, item_id = itemId };
            var content = new StringContent(
                JsonConvert.SerializeObject(payload),
                System.Text.Encoding.UTF8,
                "application/json");

            // FIX 2: Corrected casing from "statusitem_action.php" → "statusItem_action.php"
            // The server is case-sensitive — the wrong casing caused complete/restore to silently fail
            var response = await _httpClient.PutAsync($"{BaseUrl}/statusItem_action.php", content);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<object>>(json);
            return result?.Status == 200;
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Update Status Error: Request timed out.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Update Status Error: {ex.Message}");
        }
        return false;
    }

    // DELETE A TASK FROM THE SERVER
    public async Task<bool> DeleteTaskAsync(int itemId)
    {
        try
        {
            var url = $"{BaseUrl}/deleteItem_action.php?item_id={itemId}";
            var response = await _httpClient.DeleteAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<object>>(json);
            return result?.Status == 200;
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Delete Task Error: Request timed out.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Delete Task Error: {ex.Message}");
        }
        return false;
    }

    // UPDATE A TASK'S TITLE AND DETAILS ON SERVER
    public async Task<bool> EditTaskAsync(int itemId, string name, string description)
    {
        try
        {
            var payload = new { item_name = name, item_description = description, item_id = itemId };
            var content = new StringContent(
                JsonConvert.SerializeObject(payload),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PutAsync($"{BaseUrl}/editItem_action.php", content);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<object>>(json);
            return result?.Status == 200;
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Edit Task Error: Request timed out.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Edit Task Error: {ex.Message}");
        }
        return false;
    }
}

// CLASSES TO MATCH API RESPONSES
public class ApiResponse<T>
{
    public int Status { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}

public class UserData
{
    public int Id { get; set; }
    public string Fname { get; set; }
    public string Lname { get; set; }
    public string Email { get; set; }
}