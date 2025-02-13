#!/bin/bash

# Exit on error
set -e

echo "Installing Event Management Application..."

# Check if .NET SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET SDK is not installed"
    echo "Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

# Verify .NET version
DOTNET_VERSION=$(dotnet --version)
if [[ $DOTNET_VERSION != 8.* ]]; then
    echo "Error: .NET 8.0 SDK is required"
    echo "Current version: $DOTNET_VERSION"
    echo "Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

# Navigate to project directory
cd "$(dirname "$0")"

# Restore dependencies
echo "Restoring dependencies..."
dotnet restore

# Install Entity Framework CLI tools
echo "Installing Entity Framework tools..."
dotnet tool install --global dotnet-ef || true

# Create required directories
echo "Creating upload directories..."
mkdir -p src/EventApp.Web/wwwroot/uploads/images
mkdir -p src/EventApp.Web/wwwroot/uploads/documents
chmod -R 755 src/EventApp.Web/wwwroot/uploads

# Check and update configuration
if [ ! -f "src/EventApp.Web/appsettings.json" ]; then
    echo "Error: appsettings.json not found"
    exit 1
fi

# Check for Google Maps API key
MAPS_API_KEY=$(grep -o '"ApiKey": "[^"]*"' src/EventApp.Web/appsettings.json | cut -d'"' -f4)
if [ "$MAPS_API_KEY" == "YOUR_GOOGLE_MAPS_API_KEY" ]; then
    echo "Warning: Google Maps API key not configured"
    echo "Please update your API key in src/EventApp.Web/appsettings.json"
fi

# Apply database migrations
echo "Applying database migrations..."
cd src/EventApp.Web
dotnet ef database update

echo "Installation complete!"
echo ""
echo "To start the application:"
echo "1. cd src/EventApp.Web"
echo "2. dotnet run"
echo ""
echo "The application will be available at:"
echo "- Local: https://localhost:7001 or http://localhost:5000"
echo "- Network: http://<your-ip-address>:5000"