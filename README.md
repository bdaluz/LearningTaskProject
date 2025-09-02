# LearningTaskProject

This repository contains a multi-project solution for managing tasks and users using ASP.NET Core and Csharp.

## Project Structure

- `API/` - ASP.NET Core Web API for managing tasks and users
- `ProjetoTasks/` - A console application used for initial development before transitioning to the API.
- `Services/` - Business logic and data access layer

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Getting Started

### 1. Clone the Repository

```sh
git clone https://github.com/bdaluz/LearningTaskProject.git
```

### 2. Configure API Settings

#### 1. Go to the `API/` folder.
#### 2. Rename the file `example.appsettings.json` to `appsettings.json`:

#### 3. Open `API/appsettings.json` and update the configuration values (e.g., connection strings, JWT settings, email settings) with your own settings.

### 3. Create the database table

Run the following command:

```powershell
dotnet ef database update -p Services -s API
```

### 4. Restore Dependencies

Run the following command in the root folder to restore all NuGet packages:

```powershell
dotnet restore
```

### 5. Build the Solution

```powershell
dotnet build
```

### 6. Run the API Project

Navigate to the `API/` folder and run:

```powershell
dotnet run
```

The API will start and listen on the configured port.

### 7. Run the Console Application

Navigate to the `ProjetoTasks/` folder and run:

```powershell
dotnet run
```
The API will be available at `http://localhost:5075` or as configured.

## API Documentation (Swagger)

After running the API project, you can access the Swagger UI for API documentation and testing at:

- [http://localhost:5075/swagger](http://localhost:5075/swagger) (default port)
- Or check the port configured in your `launchSettings.json`.

Swagger provides a list of all available API routes and allows you to interact with them directly from the browser.

## Frontend

If you want a frontend for this project, you can find it at:

- https://github.com/bdaluz/LearningTaskProjectFrontend

## Additional Notes

- Make sure to update all configuration values in `appsettings.json` to match your environment.
- The solution targets .NET 8.0. Ensure you have the correct SDK installed.
- For database setup, check the connection string and run any required migrations.

## License

This project is for educational purposes.
