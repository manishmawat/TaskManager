using TaskManager.API.Models;
using TaskManager.API.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.ReDoc;


var builder = WebApplication.CreateBuilder(args);

// Enable CORS for localhost
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policy => policy.SetIsOriginAllowed(origin =>
            origin.StartsWith("http://localhost:") || origin == "http://localhost")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Add configuration for Cosmos DB
var cosmosSection = builder.Configuration.GetSection("CosmosDb");
var endpoint = cosmosSection["Endpoint"];
var key = cosmosSection["Key"];
var databaseId = cosmosSection["DatabaseId"] ?? "TaskManagerDb";
var containerId = cosmosSection["ContainerId"] ?? "Tasks";

builder.Services.AddOpenApi();

// Register Cosmos DB client and repository
builder.Services.AddSingleton(s =>
{
    var client = new CosmosClient(endpoint, key);
    var container = client.GetContainer(databaseId, containerId);
    return new CosmosTaskRepository(container);
});
builder.Services.AddSingleton<ITaskRepository>(sp => sp.GetRequiredService<CosmosTaskRepository>());


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "My API V1"); // Adjust the path if your OpenAPI document is at a different location
        c.RoutePrefix = "swagger"; // Customize the URL prefix for Swagger UI
    });

    app.MapScalarApiReference();

    app.UseCors("AllowLocalhost");
}

app.MapGet("/tasks", async (ITaskRepository repo) => await repo.GetAllAsync());
app.MapGet("/tasks/{id}", async (string id, ITaskRepository repo) =>
{
    var task = await repo.GetByIdAsync(id);
    return task is not null ? Results.Ok(task) : Results.NotFound();
});
app.MapPost("/tasks", async (TaskItem task, ITaskRepository repo) =>
{
    // Always auto-generate a GUID for new tasks
    task.id = Guid.NewGuid().ToString();
    var created = await repo.CreateAsync(task);
    return Results.Created($"/tasks/{created.id}", created);
});

app.MapPut("/tasks/{id}", async (string id, TaskItem task, ITaskRepository repo) =>
{
    var updated = await repo.UpdateAsync(id, task);
    return updated ? Results.NoContent() : Results.NotFound();
});

// PATCH endpoint to update only the status of a task
app.MapPatch("/tasks/{id}/status", async (string id, TaskManager.API.Models.TaskStatus status, ITaskRepository repo) =>
{
    var task = await repo.GetByIdAsync(id);
    if (task is null)
        return Results.NotFound();
    task.Status = status;
    var updated = await repo.UpdateAsync(id, task);
    return updated ? Results.Ok(task) : Results.NotFound();
});
app.MapDelete("/tasks/{id}", async (string id, ITaskRepository repo) =>
{
    var deleted = await repo.DeleteAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.Run();
