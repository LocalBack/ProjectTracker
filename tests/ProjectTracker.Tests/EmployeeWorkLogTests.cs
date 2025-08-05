using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using ProjectTracker.Data.Repositories;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Mapping;
using ProjectTracker.Service.Implementations;
using ProjectTracker.Service.Services.Implementations;
using ProjectTracker.Service.Services.Interfaces;
using ProjectTracker.Web.Authorization;
using ProjectTracker.Web.Controllers;
using Xunit;

namespace ProjectTracker.Tests;

public class NoopMediator : IMediator
{
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) => Task.FromResult(default(TResponse)!);
    public Task<object?> Send(object request, CancellationToken cancellationToken = default) => Task.FromResult<object?>(null);
    public async IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield break;
    }
    public async IAsyncEnumerable<object?> CreateStream(object request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield break;
    }
    public Task Publish(object notification, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification => Task.CompletedTask;
}

public class EmployeeWorkLogTests
{
    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        return config.CreateMapper();
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Employee_can_create_view_and_edit_own_worklog()
    {
        using var context = CreateContext();
        var mapper = CreateMapper();
        var mediator = new NoopMediator();

        // Seed data
        var user = new ApplicationUser
        {
            Id = 1,
            UserName = "employee@tracker.com",
            Email = "employee@tracker.com",
            EmployeeId = 1
        };
        var employee = new Employee
        {
            Id = 1,
            FirstName = "Test",
            LastName = "User",
            Title = "Dev",
            EmployeeCode = "E001",
            Department = "IT",
            Email = "employee@tracker.com",
            Phone = "123456789",
            HireDate = DateTime.Today
            // UserId intentionally left null to reproduce issue
        };
        var project = new Project
        {
            Id = 1,
            Name = "Proj",
            Description = "Desc",
            StartDate = DateTime.Today,
            Budget = 1000
        };
        context.Users.Add(user);
        context.Employees.Add(employee);
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        // Setup repositories and services
        var workLogRepo = new Repository<WorkLog>(context);
        var employeeRepo = new Repository<Employee>(context);
        var userRepo = new Repository<ApplicationUser>(context);
        var projectRepo = new Repository<Project>(context);

        var employeeService = new EmployeeService(employeeRepo, userRepo, mapper, mediator);
        var workLogService = new WorkLogService(workLogRepo, employeeRepo, userRepo, mapper);
        var projectService = new ProjectService(projectRepo, mapper);

        // Authorization service with custom handler
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IEmployeeService>(employeeService);
        services.AddSingleton<IAuthorizationHandler, WorkLogAuthorizationHandler>();
        services.AddAuthorization();
        var provider = services.BuildServiceProvider();
        var authorizationService = provider.GetRequiredService<IAuthorizationService>();
        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<WorkLogController>();

        // Controller with logged-in employee
        var controller = new WorkLogController(workLogService, projectService, employeeService, authorizationService, logger);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, "Employee")
                }, "Test"))
            }
        };

        // Create worklog
        var dto = new WorkLogDto
        {
            Title = "First",
            Description = "desc",
            WorkDate = DateTime.Today,
            HoursSpent = 2,
            ProjectId = project.Id
        };
        var createResult = await controller.Create(dto);
        Assert.IsType<RedirectToActionResult>(createResult);

        var worklogs = await workLogService.GetWorkLogsByEmployeeIdAsync(employee.Id);
        Assert.Single(worklogs);

        // Edit worklog
        var existing = worklogs.First();
        existing.Title = "Updated";
        var editResult = await controller.Edit(existing.Id, existing);
        Assert.IsType<RedirectToActionResult>(editResult);

        var updated = await workLogService.GetWorkLogByIdAsync(existing.Id);
        Assert.Equal("Updated", updated.Title);
    }
}
