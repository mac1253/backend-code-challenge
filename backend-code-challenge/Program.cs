using System.Text.Json;
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");


app.MapGet("/api/supervisors", async () =>
{
    var request = new HttpRequestMessage(HttpMethod.Get, "https://o3m5qixdng.execute-api.us-east-1.amazonaws.com/api/managers");
    var client = new HttpClient();

    var response = await client.SendAsync(request);
    var managers = JsonSerializer.Deserialize<List<Managers>>(response.Content.ReadAsStreamAsync().Result);

    List<String> results = new List<String>();
    // Order the list by jurisdiction then lastname and then firstname
    managers = managers.OrderBy(x => x.jurisdiction).ThenBy(x => x.lastName).ThenBy(x => x.firstName).ToList();

    foreach (var manager in managers.ToList())
    {
        int i = 0;
        // Remove managers with numerical jurisdictaion 
        if (int.TryParse(manager.jurisdiction, out i))
        {
            managers.Remove(manager);
            //Skip to next manager
            continue;
        }
        // Add formatted string to results' list
        results.Add($"{manager.jurisdiction} - {manager.lastName}, {manager.firstName}");
    }

    return results;

});

app.Run();


public class Managers
{
    public string id { get; set; }
    public string phone { get; set; }
    public string jurisdiction { get; set; }
    public string identificationNumber { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
}
