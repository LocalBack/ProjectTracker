# ProjectTracker

## Database

```bash
dotnet ef database update -p ProjectTracker.Data/ProjectTracker.Data.csproj -s ProjectTracker.Web/ProjectTracker.Web.csproj
```

## Roles

Default roles and admin/manager accounts are seeded on startup:
- `admin@projecttracker.com` / `Admin@123!`
- `manager@projecttracker.com` / `Manager@123!`

## Dashboard

![Dashboard](docs/dashboard.png)