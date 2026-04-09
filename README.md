# Ticket Desk

A ticket-based request system consisting of:

- **Backend**: Azure Functions **.NET 8** (`Ticket-Based Request System/`)
  - Uses **Azure Cosmos DB** and **Azure Blob Storage**
- **Frontend**: ASP.NET MVC **.NET Framework 4.7.2** (`TicketPlatform/`)

> **Note:** The `packages/` folder contains NuGet package artifacts and is not part of the project source.

---

## Repository Structure

```
Ticket-Based Request System/   # Azure Functions backend
TicketPlatform/                # ASP.NET MVC frontend
Ticket-Based Request System.sln
```

---

## Prerequisites

### Backend
- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Azure Functions Core Tools v4](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local)
- An Azure Cosmos DB account and Azure Blob Storage account (or local emulators via [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite))

### Frontend
- Visual Studio 2022 (recommended) with the **.NET Framework 4.7.2 targeting pack**
- IIS Express (bundled with Visual Studio) or full IIS

---

## Getting Started

### 1. Clone the repo

```bash
git clone https://github.com/A-4-Atom/ticket-desk.git
cd ticket-desk
```

### 2. Set up the backend

Create `Ticket-Based Request System/local.settings.json` (this file is gitignored and must be created manually):

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "CosmosDb:Endpoint": "<your-cosmos-endpoint>",
    "CosmosDb:Key": "<your-cosmos-key>",
    "CosmosDb:DatabaseName": "<your-database-name>",
    "BlobStorage:ConnectionString": "<your-blob-connection-string>",
    "BlobStorage:ContainerName": "<your-container-name>"
  }
}
```

Start the backend:

```bash
cd "Ticket-Based Request System"
func start
```

The Functions host will print the local URL (default: `http://localhost:7071`). Note the port — you'll need it for the frontend config.

### 3. Set up the frontend

Open `TicketPlatform/Web.config` and set `BackendBaseUrl` to match where your backend is running:

```xml
<add key="BackendBaseUrl" value="http://localhost:7071" />
```

For production, this is pre-configured to:
```xml
<add key="BackendBaseUrl" value="https://ticket-desk.azurewebsites.net" />
```

### 4. Run the frontend

1. Open `Ticket-Based Request System.sln` in Visual Studio
2. Right-click `TicketPlatform` → **Set as Startup Project**
3. Press **F5** (or click **IIS Express**) to build and launch

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend runtime | Azure Functions v4 (isolated worker), .NET 8 |
| Database | Azure Cosmos DB |
| File storage | Azure Blob Storage |
| Observability | Application Insights |
| Frontend | ASP.NET MVC 5, .NET Framework 4.7.2 |
| UI | Razor Views, jQuery, Bootstrap |

---

## Troubleshooting

- **Missing NuGet packages**: Run **Restore NuGet Packages** in Visual Studio (right-click the solution).
- **`func` not found**: Install Azure Functions Core Tools v4 and ensure it's on your `PATH`.
- **Frontend can't reach backend**: Check that `BackendBaseUrl` in `Web.config` matches the port the Functions host is listening on, and that the backend started without errors.
- **CORS errors**: If enforced on the backend, add the frontend's origin to the allowed origins in the Functions app settings or `host.json`.

