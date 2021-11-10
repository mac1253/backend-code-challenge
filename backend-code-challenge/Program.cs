// Author: Michael Corrigan

using System.ComponentModel.DataAnnotations;
using System.Net;
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

app.MapPost("/api/submit", async context =>
{
    // if content-type header in http request is not 'application/json' then return error
    if (!context.Request.HasJsonContentType())
    {
        context.Response.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
        return;
    }

    var submission = await context.Request.ReadFromJsonAsync<Submit>();
    Console.WriteLine(
          $"FirstName: {submission.firstName}\nLastName: {submission.lastName}\nSupervisor: {submission.supervisor}\nEmail: {submission.email}\nPhone: {submission.phoneNumber}"
          );

    // Model validation must be done manually using this method.
    // ref:https://docs.microsoft.com/en-us/aspnet/core/web-api/route-to-code?view=aspnetcore-6.0#notable-missing-features-compared-to-web-api

    if ((submission.firstName == null || submission.firstName == "") ||
        (submission.lastName == null || submission.lastName == "") ||
        (submission.supervisor == null || submission.supervisor == ""))
    {
        //Response 400 -  Bad Request
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return;
    }


    context.Response.StatusCode = StatusCodes.Status201Created;
    return;

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

public class Submit
{
    [Required]
    public string supervisor { get; set; }
    [Required]
    public string firstName { get; set; }
    [Required]
    public string lastName { get; set; }
    public string phoneNumber { get; set; }
    public string email { get; set; }
}
