# Event Management Application

A modern ASP.NET Core MVC application for managing events with file uploads and location features.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB or Express)
- [Git](https://git-scm.com/downloads)
- A Google Maps API key (for location features)

## Installation

### Quick Start (Script)

1. Clone the repository:
```bash
git clone https://github.com/DimitriGeelen/event-app-csharp.git
cd event-app-csharp
```

2. Run the installation script:
- For Linux/macOS:
  ```bash
  chmod +x install.sh
  ./install.sh
  ```
- For Windows:
  ```powershell
  .\install.ps1
  ```

### Manual Installation

1. Clone the repository:
```bash
git clone https://github.com/DimitriGeelen/event-app-csharp.git
cd event-app-csharp
```

2. Update the connection string in `src/EventApp.Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EventApp;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

3. Add your Google Maps API key in `appsettings.json`:
```json
{
  "GoogleMaps": {
    "ApiKey": "YOUR_API_KEY"
  }
}
```

4. Restore dependencies:
```bash
dotnet restore
```

5. Apply database migrations:
```bash
cd src/EventApp.Web
dotnet ef database update
```

6. Create required directories:
```bash
mkdir -p wwwroot/uploads/images
mkdir -p wwwroot/uploads/documents
```

## Running the Application

1. Start the application:
```bash
cd src/EventApp.Web
dotnet run
```

2. Access the application:
- Local: https://localhost:7001 or http://localhost:5000
- Network: http://<your-ip-address>:5000

## Development Setup

### Visual Studio

1. Open `EventApp.sln`
2. Restore NuGet packages
3. Update the connection string in appsettings.json
4. Run the application (F5)

### Visual Studio Code

1. Install C# Dev Kit extension
2. Open the project folder
3. Trust the workspace if prompted
4. Select the debug configuration
5. Start debugging (F5)

## Configuration

### Google Maps API Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project
3. Enable the following APIs:
   - Maps JavaScript API
   - Places API
   - Geocoding API
4. Create credentials (API key)
5. Add the API key to appsettings.json

### Database

The application uses SQL Server by default. To use a different database:

1. Install the appropriate NuGet package:
```bash
dotnet add package Microsoft.EntityFrameworkCore.PostgreSQL
# OR
dotnet add package Pomelo.EntityFrameworkCore.MySql
```

2. Update the database provider in Program.cs
3. Update the connection string in appsettings.json

## Common Issues

### Database Connection

If you see database connection errors:
1. Verify SQL Server is running
2. Check the connection string
3. Ensure the database is created:
```bash
dotnet ef database update
```

### File Permissions

If file uploads fail:
1. Check directory permissions:
```bash
# Linux/macOS
chmod -R 755 wwwroot/uploads
```
2. Ensure IIS_IUSRS has write permissions (Windows)

### Network Access

If the application isn't accessible over the network:
1. Check firewall settings
2. Allow incoming connections on ports 5000/7001
3. Update launch settings in `Properties/launchSettings.json`:
```json
{
  "applicationUrl": "http://0.0.0.0:5000;https://0.0.0.0:7001"
}
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to your branch
5. Create a pull request

## License

[MIT License](LICENSE)
