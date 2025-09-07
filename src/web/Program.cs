
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = "";
try
{
	using var client = new HttpClient();
	var configJson = await client.GetStringAsync("appsettings.json");
	var config = System.Text.Json.JsonDocument.Parse(configJson);
	apiBaseUrl = config.RootElement.GetProperty("TaskManagerApiBaseUrl").GetString();
}
catch
{
	apiBaseUrl = Environment.GetEnvironmentVariable("API_BASE_URL") ?? "http://localhost:5041/tasks";
}

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp => new TaskManager.Web.Services.TaskService(
	sp.GetRequiredService<HttpClient>(),
	apiBaseUrl
));

await builder.Build().RunAsync();
