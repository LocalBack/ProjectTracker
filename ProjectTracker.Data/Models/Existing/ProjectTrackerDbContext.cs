using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProjectTracker.Data.Models.Existing;

public partial class ProjectTrackerDbContext : DbContext
{
    public ProjectTrackerDbContext()
    {
    }

    public ProjectTrackerDbContext(DbContextOptions<ProjectTrackerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectEmployee> ProjectEmployees { get; set; }

    public virtual DbSet<WorkLog> WorkLogs { get; set; }

    public virtual DbSet<WorkLogAttachment> WorkLogAttachments { get; set; }

    public virtual DbSet<WorkLogDetail> WorkLogDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=ProjectTrackerDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Employee__3214EC0739AD2CAE");

            entity.HasIndex(e => e.Email, "IX_Employees_Email");

            entity.HasIndex(e => e.Email, "UQ_Employee_Email").IsUnique();

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Projects__3214EC078A2E02A2");

            entity.HasIndex(e => e.StartDate, "IX_Projects_StartDate");

            entity.Property(e => e.ActualCost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Budget).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<ProjectEmployee>(entity =>
        {
            entity.HasKey(e => new { e.ProjectId, e.EmployeeId });

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Role).HasMaxLength(100);

            entity.HasOne(d => d.Employee).WithMany(p => p.ProjectEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectEmployees_Employees");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectEmployees)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectEmployees_Projects");
        });

        modelBuilder.Entity<WorkLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WorkLogs__3214EC07074B2085");

            entity.HasIndex(e => e.EmployeeId, "IX_WorkLogs_EmployeeId");

            entity.HasIndex(e => e.ProjectId, "IX_WorkLogs_ProjectId");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.HoursSpent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Employee).WithMany(p => p.WorkLogs)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkLogs_Employees");

            entity.HasOne(d => d.Project).WithMany(p => p.WorkLogs)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkLogs_Projects");
        });

        modelBuilder.Entity<WorkLogAttachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WorkLogA__3214EC07CD0F37F3");

            entity.HasIndex(e => e.WorkLogId, "IX_WorkLogAttachments_WorkLogId");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.WorkLog).WithMany(p => p.WorkLogAttachments)
                .HasForeignKey(d => d.WorkLogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkLogAttachments_WorkLogs");
        });

        modelBuilder.Entity<WorkLogDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WorkLogD__3214EC077240E7FC");

            entity.HasIndex(e => e.WorkLogId, "IX_WorkLogDetails_WorkLogId");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.WorkLog).WithMany(p => p.WorkLogDetails)
                .HasForeignKey(d => d.WorkLogId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkLogDetails_WorkLogs");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
