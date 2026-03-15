using Newtonsoft.Json; // This requires the NuGet package you just installed
using ToDoApplication.Models;

namespace ToDoApplication.Services;

public static class StorageService
{
    // This is the hidden folder on the phone where data is safe
    private static string FilePath => Path.Combine(FileSystem.AppDataDirectory, "user_tasks.json");
    public static User CurrentUser { get; set; }

    public static List<User> LoadAllUsers()
    {
        if (!File.Exists(FilePath)) return new List<User>();
        try 
        {
            var json = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
        } 
        catch { return new List<User>(); }
    }

    public static void SaveCurrentState()
    {
        if (CurrentUser == null) return;
        
        var allUsers = LoadAllUsers();
        var index = allUsers.FindIndex(u => u.Email == CurrentUser.Email);
        
        if (index != -1) allUsers[index] = CurrentUser;
        else allUsers.Add(CurrentUser);

        var json = JsonConvert.SerializeObject(allUsers);
        File.WriteAllText(FilePath, json);
    }
}