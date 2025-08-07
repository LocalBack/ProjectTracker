# ProjectTracker

## Dashboard Setup

After cloning the repository run the database migrations:

```bash
dotnet ef database update
```

The dashboard views rely on CDN hosted versions of Chart.js and DataTables
with the Buttons extension.  No additional installation is required.

Seeded roles include `Admin`, `Manager` and `Employee`.  Ensure the seeding
step runs on first start to create these roles and default users.