# PowerShell installation script for Event Management Application

Write-Host "Installing Event Management Application..." -ForegroundColor Green

# Check if .NET SDK is installed
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "Error: .NET SDK is not installed" -ForegroundColor Red
    Write-Host "Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
}

# Verify .NET version
$dotnetVersion = dotnet --version
if (-not ($dotnetVersion -match "^8\.")) {
    Write-Host "Error: .NET 8.0 SDK is required" -ForegroundColor Red
    Write-Host "Current version: $dotnetVersion"
    Write-Host "Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
}

# Navigate to project directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

# Restore dependencies
Write-Host "Restoring dependencies..." -ForegroundColor Yellow
dotnet restore

# Install Entity Framework CLI tools
Write-Host "Installing Entity Framework tools..." -ForegroundColor Yellow
dotnet tool install --global dotnet-ef

# Create required directories
Write-Host "Creating upload directories..." -ForegroundColor Yellow
$uploadsPath = "src\EventApp.Web\wwwroot\uploads"
New-Item -ItemType Directory -Force -Path "$uploadsPath\images" | Out-Null
New-Item -ItemType Directory -Force -Path "$uploadsPath\documents" | Out-Null

# Check and update configuration
$settingsPath = "src\EventApp.Web\appsettings.json"
if (-not (Test-Path $settingsPath)) {
    Write-Host "Error: appsettings.json not found" -ForegroundColor Red
    exit 1
}

# Check for Google Maps API key
$settings = Get-Content $settingsPath | ConvertFrom-Json
if ($settings.GoogleMaps.ApiKey -eq "YOUR_GOOGLE_MAPS_API_KEY") {
    Write-Host "Warning: Google Maps API key not configured" -ForegroundColor Yellow
    Write-Host "Please update your API key in $settingsPath"
}

# Check SQL Server LocalDB
Write-Host "Checking SQL Server LocalDB..." -ForegroundColor Yellow
$localDb = Get-WmiObject -Class Win32_Service -Filter "Name='SQLWriter'" -ErrorAction SilentlyContinue
if (-not $localDb) {
    Write-Host "Warning: SQL Server LocalDB might not be installed" -ForegroundColor Yellow
    Write-Host "Please install SQL Server LocalDB from https://go.microsoft.com/fwlink/?LinkID=866658"
    Write-Host "Or update the connection string in appsettings.json to use a different database"
}

# Apply database migrations
Write-Host "Applying database migrations..." -ForegroundColor Yellow
Set-Location "src\EventApp.Web"
dotnet ef database update
Set-Location ..\..

Write-Host "`nInstallation complete!" -ForegroundColor Green
Write-Host "`nTo start the application:"
Write-Host "1. cd src\EventApp.Web"
Write-Host "2. dotnet run"
Write-Host "`nThe application will be available at:"
Write-Host "- Local: https://localhost:7001 or http://localhost:5000"
Write-Host "- Network: http://<your-ip-address>:5000"

# Optional: Setup IIS
$setupIIS = Read-Host "`nWould you like to configure IIS for this application? (y/N)"
if ($setupIIS -eq 'y') {
    Write-Host "Checking IIS features..." -ForegroundColor Yellow
    
    # Check if running as administrator
    $currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
    if (-not $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
        Write-Host "Error: IIS setup requires administrator privileges" -ForegroundColor Red
        Write-Host "Please run this script as administrator to configure IIS"
        exit 1
    }
    
    # Install IIS features
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-CommonHttpFeatures
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpErrors
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationDevelopment
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-NetFxExtensibility45
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HealthAndDiagnostics
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpLogging
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-Security
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-RequestFiltering
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-Performance
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerManagementTools
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-WindowsAuthentication
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-StaticContent
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-DefaultDocument
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ASPNET45
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ISAPIExtensions
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ISAPIFilter
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpCompressionStatic
    
    Write-Host "IIS features installed successfully" -ForegroundColor Green
    Write-Host "Please use IIS Manager to create a new website for the application"
}