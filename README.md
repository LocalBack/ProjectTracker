# ProjectTracker

ProjectTracker is a multi-project ASP.NET Core solution for tracking projects, work logs and employees. It targets **.NET 8** and uses Entity Framework Core, Identity and AutoMapper.

## Solution structure

- **ProjectTracker.Web** - MVC front‑end for end users.
- **ProjectTracker.Admin** - administrative site with Identity UI.
- **ProjectTracker.Service** - business services and DTOs.
- **ProjectTracker.Data** - EF Core context, repositories and migrations.
- **ProjectTracker.Core** - domain entities, enums and events.

## Requirements

- .NET 8 SDK
- SQL Server instance

## Getting started

1. Restore packages and build the solution:

   ```bash
   dotnet restore
   dotnet build ProjectTracker.sln
   ```
2. Apply migrations (adjust connection string in `appsettings.json` if needed):

   ```bash
   dotnet ef database update --project ProjectTracker.Data
   ```
3. Seed initial roles and the admin user. The `IdentitySeed` class is executed at startup.
4. Run the applications:

   ```bash
   dotnet run --project ProjectTracker.Web   # main site
   dotnet run --project ProjectTracker.Admin # admin console
   ```

## Contributing

- Follow the coding style used in the existing code (4‑space indentation and opening braces on new lines).
- Verify that `dotnet build ProjectTracker.sln` succeeds before committing.
- Include clear commit messages summarising the change.

Feel free to open issues or pull requests if you encounter problems.
