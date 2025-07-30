# ProjectTracker

## Overview
ProjectTracker is a multi-project tracking system built with ASP.NET Core 8. It provides tools to manage projects, employees and work logs. The solution contains two applications:

- **ProjectTracker.Web** – the main site where employees record their work and managers view reports.
- **ProjectTracker.Admin** – an admin panel for managing users and projects. A dedicated **Task Manager** will be added here soon.

## Setup
1. **Prerequisites**
   - [.NET 8 SDK](https://dotnet.microsoft.com/) installed
   - SQL Server instance available for the connection string `DefaultConnection`

2. **Restore and build**
   ```bash
   dotnet restore
   dotnet build
   ```

3. **Database**
   - Update `DefaultConnection` in `ProjectTracker.Web/appsettings.json` and `ProjectTracker.Admin/appsettings.json`.
   - Apply migrations from the `ProjectTracker.Data` project:
     ```bash
     dotnet ef database update --project ProjectTracker.Data
     ```
   - Running either web application will also seed an administrator account (`admin@projecttracker.com` / `Admin@123!`).

4. **Run the applications**
   ```bash
   # public site
   dotnet run --project ProjectTracker.Web

   # admin panel
   dotnet run --project ProjectTracker.Admin
   ```

## Admin Panel Features
The admin site provides tools for system administrators:

- **User Management** – create, edit and delete accounts, assign roles (Admin, Manager, Employee, ReadOnly).
- **Project Management** – create and maintain projects.
- **Task Manager** – *coming soon*. This section will allow admins to create tasks and assign them to users within each project, track status and deadlines.

The admin panel enforces authentication and authorization using ASP.NET Core Identity. Only users in the `Admin` role can access the pages.

---
When the Task Manager is implemented, this README will be updated with usage instructions and screenshots.
