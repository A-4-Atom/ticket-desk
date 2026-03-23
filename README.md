# Ticket Desk

A ticket-based request system with:

- **Backend**: Azure Functions **.NET 8** (`Ticket-Based Request System/`)
  - Uses **Azure Cosmos DB** and **Azure Blob Storage**
- **Frontend**: ASP.NET MVC **.NET Framework 4.7.2** web app (`TicketPlatform/`)

> Note: This repo currently contains a `packages/` folder with NuGet package contents (e.g., `packages/Newtonsoft.Json.13.0.3/README.md`). Those are third-party artifacts and not the project’s main README.

---

## Repository structure

- `Ticket-Based Request System/` — Azure Functions backend (`backend.csproj`, `Program.cs`, `host.json`)
- `TicketPlatform/` — ASP.NET MVC frontend (`frontend.csproj`, `Web.config`, Controllers/Views/etc.)
- `Ticket-Based Request System.sln` — Visual Studio solution

---

## Prerequisites

### Backend (Azure Functions - .NET 8)
- **.NET SDK 8.0**
- **Azure Functions Core Tools v4** (recommended for local run/debug)
- Access to (or local emulators/mocked equivalents of):
  - Azure Cosmos DB
  - Azure Blob Storage

### Frontend (ASP.NET MVC - .NET Framework 4.7.2)
- **Visual Studio** (recommended) with **.NET Framework 4.7.2** targeting pack installed
- IIS Express (bundled with Visual Studio) or IIS

---

## Getting started (local development)

### 1) Clone
```bash
git clone https://github.com/A-4-Atom/ticket-desk.git
cd ticket-desk
```

---

## Backend (Azure Functions) — run locally

The backend project is:
- `Ticket-Based Request System/backend.csproj` (targets `net8.0`, Azure Functions v4)

### Configure settings
The project references `local.settings.json` (it’s included in the `.csproj`), but the file is not present at the repo root in GitHub (it likely should be created locally and kept out of git).

Create:
- `Ticket-Based Request System/local.settings.json`

Example template (adjust keys to what your functions/services expect):
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",

    "CosmosDb:Endpoint": "",
    "CosmosDb:Key": "",
    "CosmosDb:DatabaseName": "",

    "BlobStorage:ConnectionString": "",
    "BlobStorage:ContainerName": ""
  }
}
```

### Run
From the repo root:
```bash
cd "Ticket-Based Request System"
func start
```

If you prefer using the .NET CLI (may work depending on your setup):
```bash
cd "Ticket-Based Request System"
dotnet build
dotnet run
```

---

## Frontend (ASP.NET MVC) — run locally

The frontend project is:
- `TicketPlatform/frontend.csproj` (targets `.NET Framework 4.7.2`)

### Configure backend URL
The frontend reads the backend base URL from:
- `TicketPlatform/Web.config`

Key:
- `BackendBaseUrl` (currently set to `http://localhost:7255`)

```xml
<add key="BackendBaseUrl" value="http://localhost:7255" />
```

Make sure this matches the URL/port your Functions backend is running on.

### Run (Visual Studio)
1. Open `Ticket-Based Request System.sln` in Visual Studio
2. Set `TicketPlatform` as the startup project
3. Run with **IIS Express**

---

## Tech stack

### Backend
- Azure Functions v4 (isolated worker)
- .NET 8
- Azure Cosmos DB SDK
- Azure Storage Blobs SDK
- Application Insights (worker service)

### Frontend
- ASP.NET MVC 5 (.NET Framework 4.7.2)
- Razor Views
- jQuery + Bootstrap (static assets under `TicketPlatform/Scripts` and `TicketPlatform/Content`)

---

## Notes / troubleshooting

- If NuGet packages are missing for the frontend, run a **NuGet restore** in Visual Studio.
- For the backend, ensure **Azure Functions Core Tools v4** is installed and available on your PATH.
- If the frontend can’t reach the backend, verify:
  - `BackendBaseUrl` in `TicketPlatform/Web.config`
  - Backend is actually listening on that port
  - CORS/auth requirements (if enforced in backend endpoints)

---

## License

No license file was detected at the repository root. If you intend this project to be open source, add a `LICENSE` file (e.g., MIT, Apache-2.0, GPL-3.0, etc.).