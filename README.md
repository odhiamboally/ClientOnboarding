# Client Onboarding Application

This is a .NET 10 Blazor application designed for client onboarding.

## Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A modern web browser (Chrome, Firefox, Edge)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or any supported database

## Instructions for Running the App
1. Clone the repository:
   ```bash
   git clone https://github.com/odhiamboally/ClientOnboarding.git
   cd ClientOnboarding
   ```
2. Restore the required packages:
   ```bash
   dotnet restore
   ```
3. Run database migrations:
   ```bash
   dotnet ef database update
   ```
4. Start the application:
   ```bash
   dotnet run
   ```
5. Open your browser and navigate to `http://localhost:5000`.

## Migration Instructions
- Ensure you have the `dotnet-ef` tool installed:
   ```bash
   dotnet tool install --global dotnet-ef
   ```
- To create a migration, use:
   ```bash
   dotnet ef migrations add <MigrationName>
   ```
- Once the migration is created, update the database with:
   ```bash
   dotnet ef database update
   ```