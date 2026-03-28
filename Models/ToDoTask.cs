using Newtonsoft.Json;

namespace ToDoApplication.Models;

public class TodoTask
{
    // The server gives us an integer ID, not a string Guid
    [JsonProperty("item_id")]
    public int Id { get; set; }

    [JsonProperty("item_name")]
    public string Title { get; set; }

    [JsonProperty("item_description")]
    public string Details { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; } // The server uses "active" or "inactive"

    [JsonProperty("user_id")]
    public int UserId { get; set; }

    // This is a helper property so your UI checkboxes still work easily
    // We tell JSON to ignore this because we don't send "IsCompleted" to the server
    [JsonIgnore]
    public bool IsCompleted => Status == "inactive";
}