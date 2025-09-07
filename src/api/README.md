# Task Manager API

This project is a .NET 9 Minimal API for managing tasks. It uses Cosmos DB by default, but is designed to allow switching to other database providers via configuration.

## Features
- Minimal API (Program.cs only)
- Basic CRUD endpoints for tasks
- Cosmos DB integration (default)
- Configurable repository provider

## Structure
- All API code is under `src/api`

## Getting Started
1. Ensure you have .NET 9 SDK installed.
2. Update `appsettings.json` with your database configuration.
3. Run the API:
   ```pwsh
   dotnet run --project src/api
   ```

## Endpoints
- `GET /tasks` - List all tasks
- `GET /tasks/{id}` - Get a task by ID
- `POST /tasks` - Create a new task
- `PUT /tasks/{id}` - Update a task
- `DELETE /tasks/{id}` - Delete a task

## Switching Database Providers
Edit `appsettings.json` to change the database provider and connection details. The repository is injected via DI based on configuration.

---

This README will be updated as the project is scaffolded and implemented.
