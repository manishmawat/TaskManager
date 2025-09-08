# TaskManager

This repository contains the TaskManager API and web application.

## Project Structure

- `src/api/` - .NET API for managing tasks
- `src/web/` - Web frontend (add details if implemented)

## Getting Started

### Prerequisites
- .NET 9 SDK
- Docker (for containerization)
- Azure account (for deployment)

### Local Development
1. Clone the repository:
   ```sh
   git clone https://github.com/manishmawat/TaskManager.git
   ```
2. Navigate to the API folder:
   ```sh
   cd src/api
   ```
3. Run the API:
   ```sh
   dotnet run
   ```

### Deployment
This project uses GitHub Actions to deploy the API as an Azure Container App. See `.github/workflows/TaskManagerAPI.yml` for details.

#### Required Azure Resources
- Azure Container Registry
- Azure Container App
- Azure Cosmos DB
- Resource Group

#### Secrets
Set the following secrets in your GitHub repository:
- `AZURE_CREDENTIALS`
- `COSMOSDB_ENDPOINT`
- `COSMOSDB_KEY`
- `COSMOSDB_DATABASEID`
- `COSMOSDB_CONTAINERID`

## License
MIT
