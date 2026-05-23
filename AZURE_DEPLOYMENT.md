# Azure Deployment Guide

This guide explains how to deploy the ClinicOS application to Azure free tier web apps.

## Prerequisites

- Azure account with free tier
- Azure CLI installed (optional)
- Git

## Architecture

- **API**: ASP.NET Core Web API deployed to Azure App Service
- **UI**: React/Vite application deployed to Azure Static Web App or App Service

## API Deployment

### 1. Create Azure SQL Database

```bash
# Create a resource group
az group create --name ClinicOS-RG --location eastus

# Create a SQL server
az sql server create --name clinicos-sql-server --resource-group ClinicOS-RG --location eastus --admin-user your-admin --admin-password your-password

# Create a SQL database
az sql db create --name ClinicOSDb --server clinicos-sql-server --resource-group ClinicOS-RG --edition Basic --capacity 5
```

### 2. Create Azure App Service for API

```bash
# Create an App Service plan (free tier)
az appservice plan create --name ClinicOS-API-Plan --resource-group ClinicOS-RG --sku FREE

# Create the web app
az webapp create --name clinicos-api --resource-group ClinicOS-RG --plan ClinicOS-API-Plan
```

### 3. Configure Environment Variables

Set the following environment variables in your Azure Web App (Settings > Configuration > Application Settings):

```
AZURE_SQL_CONNECTION_STRING=Server=tcp:clinicos-sql-server.database.windows.net,1433;Database=ClinicOSDb;User ID=your-admin@clinicos-sql-server;Password=your-password;Encrypt=true;Connection Timeout=30;TrustServerCertificate=False;
JWT_SECRET_KEY=your-secure-random-jwt-key-here
API_KEY=your-api-key-here
ASPNETCORE_ENVIRONMENT=Production
```

**Important**: Replace the placeholder values with your actual values.

### 4. Deploy the API

Option 1: Using Visual Studio
1. Right-click the ClinicOS.API project
2. Publish > Azure > Azure App Service
3. Select your created web app
4. Click Publish

Option 2: Using Azure CLI
```bash
# Build the project
dotnet publish ClinicOS.API/ClinicOS.API.csproj -c Release -o ./publish

# Deploy to Azure
az webapp up --name clinicos-api --resource-group ClinicOS-RG --location eastus --sku FREE
```

### 5. Run Database Migrations

You'll need to run migrations on the production database. You can do this by:
- Using EF Core tools locally with the production connection string
- Or enabling automatic migrations in production (not recommended for production)

```bash
# Run migrations locally with production connection string
dotnet ef database update --connection "Your-Production-Connection-String"
```

## UI Deployment

### 1. Build the UI for Production

Update `.env.production` with your API URL:
```
VITE_API_BASE_URL=https://clinicos-api.azurewebsites.net/api
```

Build the application:
```bash
cd AIDentalUI
npm run build
```

### 2. Create Azure Static Web App (Recommended for React)

```bash
# Create a Static Web App
az staticwebapp create --name clinicos-ui --resource-group ClinicOS-RG --location eastus --sku Free --source AIDentalUI --branch main --app-location . --api-location "" --output-location dist
```

Or deploy to Azure App Service:
```bash
# Create an App Service plan (free tier)
az appservice plan create --name ClinicOS-UI-Plan --resource-group ClinicOS-RG --sku FREE

# Create the web app
az webapp create --name clinicos-ui --resource-group ClinicOS-RG --plan ClinicOS-UI-Plan
```

### 3. Deploy the UI

Option 1: Using Azure Static Web App with GitHub
1. Push your code to GitHub
2. In Azure portal, create a Static Web App
3. Connect it to your GitHub repository
4. Configure build settings:
   - App location: `/`
   - Api location: (leave empty)
   - Output location: `dist`

Option 2: Using Zip Deploy to App Service
```bash
# Build the UI
npm run build

# Create a zip file
cd dist
zip -r ../ui-dist.zip .
cd ..

# Deploy to Azure
az webapp deployment source config-zip --resource-group ClinicOS-RG --name clinicos-ui --src ui-dist.zip
```

## Environment Variables Summary

### API Environment Variables (appsettings.Production.json)

The production settings use these environment variables:
- `AZURE_SQL_CONNECTION_STRING`: Your Azure SQL connection string
- `JWT_SECRET_KEY`: A secure random key for JWT token generation
- `API_KEY`: Your API key for API authentication

### UI Environment Variables (.env.production)

- `VITE_API_BASE_URL`: The base URL of your deployed API (e.g., `https://clinicos-api.azurewebsites.net/api`)

## Important Notes

1. **Security**: Never commit actual connection strings or secrets to your repository. Use environment variables in Azure.
2. **CORS**: You may need to configure CORS in your API to allow requests from your UI domain.
3. **HTTPS**: Azure provides HTTPS automatically for all App Services.
4. **Database Backups**: Enable automated backups for your Azure SQL database.
5. **Scaling**: The free tier has limitations. Monitor your usage and upgrade if needed.

## CORS Configuration

Add your UI domain to the CORS configuration in your API's `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy => policy
            .WithOrigins("https://clinicos-ui.azurewebsites.net")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
```

## Troubleshooting

### API not responding
- Check the logs in Azure portal (Log Stream)
- Verify environment variables are set correctly
- Ensure the database connection string is valid

### UI not connecting to API
- Verify `VITE_API_BASE_URL` is correct
- Check browser console for CORS errors
- Ensure API is running and accessible

### Database connection issues
- Verify the SQL server firewall allows Azure services
- Check the connection string format
- Ensure the database exists and migrations have been run
