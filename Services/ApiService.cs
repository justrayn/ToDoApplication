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
        _httpClient = new HttpClient();
    }

    // STEP A: SIGN IN
    public async Task<User> SignInAsync(string email, string password)
    {
        try 
        {
            var url = $"{BaseUrl}/signin_action.php?email={email}&password={password}";
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
            }
        }
        catch (Exception ex) { Console.WriteLine($"Sign In Error: {ex.Message}"); }
        return null;
    }

    // STEP B: SIGN UP 
    public async Task<bool> SignUpAsync(string firstName, string lastName, string email, string password)
    {
        try 
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("first_name", firstName),
                new KeyValuePair<string, string>("last_name", lastName),
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("confirm_password", password)
            });

            var response = await _httpClient.PostAsync($"{BaseUrl}/signup_action.php", formContent);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<object>>(json);

            return result?.Status == 200;
        }
        catch (Exception ex) 
        { 
            Console.WriteLine($"Sign Up Error: {ex.Message}"); 
            return false; 
        }
    }

    // --- NEW METHODS BELOW ---

    // FETCH TASKS FROM SERVER
    // FETCH TASKS FROM SERVER
    public async Task<List<TodoTask>> GetTasksAsync(int userId, string status)
    {
        try 
        {
            // Note: Make sure the exact casing matches your professor's server (getItems vs getitems)
            var url = $"{BaseUrl}/getItems_action.php?status={status}&user_id={userId}";
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                
                // Print exactly what the server sends back so we can see it!
                Console.WriteLine($"--- GET TASKS JSON ({status}): {json} ---");
                
                // Read the raw JSON data flexibly using JToken
                var result = JsonConvert.DeserializeObject<ApiResponse<JToken>>(json);
                
                if (result?.Status == 200 && result.Data != null)
                {
                    // If the server sends a List [ ]
                    if (result.Data.Type == JTokenType.Array)
                    {
                        return result.Data.ToObject<List<TodoTask>>() ?? new List<TodoTask>();
                    }
                    // If the server sends a Dictionary { "0": { ... } }
                    else if (result.Data.Type == JTokenType.Object)
                    {
                        var dict = result.Data.ToObject<Dictionary<string, TodoTask>>();
                        return dict?.Values.ToList() ?? new List<TodoTask>();
                    }
                }
            }
        }
        catch (Exception ex) 
        { 
            Console.WriteLine($"Get Tasks Error: {ex.Message}"); 
        }
        
        return new List<TodoTask>();
    }

    // ADD A TASK TO THE SERVER
    // ADD A TASK TO THE SERVER
    public async Task<bool> AddTaskAsync(string name, string description, int userId)
    {
        try
        {
            // 1. Pack the data as a JSON object instead of a web form
            var payload = new 
            { 
                item_name = name, 
                item_description = description, 
                user_id = userId 
            };
            
            var content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");

            // 2. Send it to the server
            var response = await _httpClient.PostAsync($"{BaseUrl}/addItem_action.php", content);
            var json = await response.Content.ReadAsStringAsync();
            
            // 3. Print the server's exact reply to your Visual Studio Output window
            Console.WriteLine($"--- ADD TASK SERVER RESPONSE: {json} ---");
            
            var result = JsonConvert.DeserializeObject<ApiResponse<object>>(json);

            return result?.Status == 200;
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
            var content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{BaseUrl}/statusitem_action.php", content);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<object>>(json);

            return result?.Status == 200;
        }
        catch (Exception ex) { Console.WriteLine($"Update Status Error: {ex.Message}"); }
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
        catch (Exception ex) { Console.WriteLine($"Delete Task Error: {ex.Message}"); }
        return false;
    }
    
    // UPDATE A TASK'S TITLE AND DETAILS ON SERVER
    public async Task<bool> EditTaskAsync(int itemId, string name, string description)
    {
        try
        {
            var payload = new { item_name = name, item_description = description, item_id = itemId };
            var content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{BaseUrl}/editItem_action.php", content);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse<object>>(json);

            return result?.Status == 200;
        }
        catch (Exception ex) { Console.WriteLine($"Edit Task Error: {ex.Message}"); }
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