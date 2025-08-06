using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectTracker.Core.Entities;

namespace ProjectTracker.Data.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Domain Entities
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<WorkLog> WorkLogs { get; set; } = null!;
        public DbSet<WorkLogDetail> WorkLogDetails { get; set; } = null!;
        public DbSet<WorkLogAttachment> WorkLogAttachments { get; set; } = null!;
        public DbSet<WorkLogHistory> WorkLogHistories { get; set; } = null!;
        public DbSet<ProjectEmployee> ProjectEmployees { get; set; } = null!;
        public DbSet<Equipment> Equipments { get; set; } = null!;
        public DbSet<MaintenanceSchedule> MaintenanceSchedules { get; set; } = null!;
        public DbSet<MaintenanceLog> MaintenanceLogs { get; set; } = null!;
        public DbSet<ProjectDocument> ProjectDocuments { get; set; } = null!;

        // Identity Extension
        public DbSet<UserProject> UserProjects { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Identity base configuration - ÇOK ÖNEMLİ!
            base.OnModelCreating(modelBuilder);

            // Project Configuration
            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Budget)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.ActualCost)
                    .HasColumnType("decimal(18,2)");

                entity.HasIndex(e => e.Name);
            });

            // Employee Configuration
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Change from:
                // entity.Property(e => e.Name)...     // Line 59
                // entity.Property(e => e.Position)... // Line 69

                // To:
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Title)      // Changed from Position
                    .HasMaxLength(100);

                entity.Property(e => e.EmployeeCode)
                    .HasMaxLength(50);

                entity.Property(e => e.Department)
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .HasMaxLength(100);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20);

                entity.Property(e => e.HireDate)
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                // Configure the relationship with ApplicationUser
                entity.HasOne(e => e.User)
                    .WithOne()
                    .HasForeignKey<Employee>(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Configure relationships
                entity.HasMany(e => e.WorkLogs)
                    .WithOne(w => w.Employee)
                    .HasForeignKey(w => w.EmployeeId);

                entity.HasMany(e => e.ProjectEmployees)
                    .WithOne(pe => pe.Employee)
                    .HasForeignKey(pe => pe.EmployeeId);
            });

            // WorkLog Configuration
            modelBuilder.Entity<WorkLog>(entity =>
            {
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.HoursSpent)
                    .HasColumnType("decimal(5,2)");

                entity.Property(e => e.Cost)
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

                entity.HasOne(e => e.Project)
                    .WithMany(p => p.WorkLogs)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Employee)
                    .WithMany(emp => emp.WorkLogs)
                    .HasForeignKey(e => e.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.WorkDate);
                entity.HasIndex(e => new { e.ProjectId, e.EmployeeId });
            });

            // WorkLogDetail Configuration
            modelBuilder.Entity<WorkLogDetail>(entity =>
            {
                entity.Property(e => e.StepDescription)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.TechnicalDetails)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Result)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.AdditionalData)
                    .HasColumnType("nvarchar(max)");

                entity.HasOne(e => e.WorkLog)
                    .WithMany(w => w.Details)
                    .HasForeignKey(e => e.WorkLogId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // WorkLogAttachment Configuration
            modelBuilder.Entity<WorkLogAttachment>(entity =>
            {
                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.FileType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.HasOne(e => e.WorkLog)
                    .WithMany(w => w.Attachments)
                    .HasForeignKey(e => e.WorkLogId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ProjectDocument Configuration
            modelBuilder.Entity<ProjectDocument>(entity =>
            {
                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.FilePath)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.FileType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.HasOne(e => e.Project)
                    .WithMany(p => p.Documents)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // WorkLogHistory Configuration
            modelBuilder.Entity<WorkLogHistory>(entity =>
            {
                entity.ToTable("WorkLogHistory");
                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ChangedByUserName)
                    .HasMaxLength(100);

                entity.Property(e => e.Changes)
                    .HasColumnType("nvarchar(max)");

                entity.HasOne(e => e.WorkLog)
                    .WithMany(w => w.History)
                    .HasForeignKey(e => e.WorkLogId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ProjectEmployee Configuration (Many-to-Many)
            modelBuilder.Entity<ProjectEmployee>(entity =>
            {
                entity.HasKey(pe => new { pe.ProjectId, pe.EmployeeId });

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(pe => pe.Project)
                    .WithMany(p => p.ProjectEmployees)
                    .HasForeignKey(pe => pe.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(pe => pe.Employee)
                    .WithMany(e => e.ProjectEmployees)
                    .HasForeignKey(pe => pe.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.ProjectId, e.IsActive });
            });

            // Equipment Configuration
            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.SerialNumber)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(e => e.SerialNumber)
                    .IsUnique();

                entity.HasOne(e => e.Project)
                    .WithMany(p => p.Equipments)
                    .HasForeignKey(e => e.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // MaintenanceSchedule Configuration
            modelBuilder.Entity<MaintenanceSchedule>(entity =>
            {
                entity.Property(e => e.MaintenanceType)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(e => e.Equipment)
                    .WithMany(eq => eq.MaintenanceSchedules)
                    .HasForeignKey(e => e.EquipmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.NextMaintenanceDate);
            });

            // MaintenanceLog Configuration
            modelBuilder.Entity<MaintenanceLog>(entity =>
            {
                entity.ToTable("MaintenanceLogs");

                entity.Property(e => e.MaintenanceDate)
                    .IsRequired();

                entity.Property(e => e.PerformedBy)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Notes)
                    .HasMaxLength(2000);

                entity.Property(e => e.Cost)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.IsCompleted)
                    .IsRequired();

                entity.HasOne(e => e.MaintenanceSchedule)
                    .WithMany()
                    .HasForeignKey(e => e.MaintenanceScheduleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.MaintenanceScheduleId);
            });
            // ------------------------------------------------

            // ApplicationUser Configuration
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                // User-Employee ilişkisi (One-to-One)
                entity.HasOne(u => u.Employee)
                    .WithOne()
                    .HasForeignKey<ApplicationUser>(u => u.EmployeeId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);

                entity.HasIndex(e => e.EmployeeId)
                    .IsUnique()
                    .HasFilter("[EmployeeId] IS NOT NULL");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.KVKK)
                    .HasColumnName("KVKK")
                    .HasDefaultValue(false);

                entity.Property(e => e.KvkkTimestamp)
                    .HasColumnName("KVKK_Timestamp");
            });

            // UserProject Configuration (User-Project Many-to-Many)
            modelBuilder.Entity<UserProject>(entity =>
            {
                entity.HasKey(up => new { up.UserId, up.ProjectId });

                entity.HasOne(up => up.User)
                    .WithMany(u => u.UserProjects)
                    .HasForeignKey(up => up.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(up => up.Project)
                    .WithMany()
                    .HasForeignKey(up => up.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.CanView });
                entity.HasIndex(e => e.ProjectId);
            });

            // ApplicationRole Configuration
            modelBuilder.Entity<ApplicationRole>(entity =>
            {
                entity.Property(e => e.Description)
                    .HasMaxLength(250);
            });

            // Global Query Filters (Soft Delete için)
            modelBuilder.Entity<Project>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<Employee>().HasQueryFilter(e => e.IsActive);
            modelBuilder.Entity<WorkLog>().HasQueryFilter(e => e.IsActive);
        }

        // SaveChanges override - CreatedDate ve UpdatedDate otomatik set etmek için
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.IsActive = true;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedDate = DateTime.Now;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.IsActive = true;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedDate = DateTime.Now;
                        break;
                }
            }

            return base.SaveChanges();
        }
    }
}