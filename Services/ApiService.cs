using System.Net.Http.Json;
using ToDoApplication.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;

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
                        Id = apiResponse.Data.Id, // Grabs the ID
                        Email = apiResponse.Data.Email,
                        Username = $"{apiResponse.Data.Fname} {apiResponse.Data.Lname}" 
                    };
                }
            }
        }
        catch (Exception ex) { Console.WriteLine($"Sign In Error: {ex.Message}"); }
        return null;
    }

    // STEP B: SIGN UP (Using Web Form format for PHP)
    public async Task<bool> SignUpAsync(string firstName, string lastName, string email, string password)
    {
        try 
        {
            // Pack the data exactly how PHP likes it
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
            
            Console.WriteLine($"--- SERVER SAID: {json} ---");

            var result = JsonConvert.DeserializeObject<ApiResponse<object>>(json);

            return result?.Status == 200;
        }
        catch (Exception ex) 
        { 
            Console.WriteLine($"Sign Up Error: {ex.Message}"); 
            return false; 
        }
    }
}

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