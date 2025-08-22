using Microsoft.EntityFrameworkCore;
using PerformanceEvaluation.Domain.Entities;
using PerformanceEvaluation.Infrastructure.Configurations;

namespace PerformanceEvaluation.Infrastructure.Data;

public class PerformanceEvaluationDbContext : DbContext
{
    public PerformanceEvaluationDbContext(DbContextOptions<PerformanceEvaluationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Competency> Competencies { get; set; }
    public DbSet<EvaluationSession> EvaluationSessions { get; set; }
    public DbSet<EvaluationResult> EvaluationResults { get; set; }
    public DbSet<AnonymousMapping> AnonymousMappings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        modelBuilder.ApplyConfiguration(new CompetencyConfiguration());
        modelBuilder.ApplyConfiguration(new EvaluationSessionConfiguration());
        modelBuilder.ApplyConfiguration(new EvaluationResultConfiguration());
        modelBuilder.ApplyConfiguration(new AnonymousMappingConfiguration());

        // Global query filters and indexes
        ConfigureIndexes(modelBuilder);
        ConfigureGlobalFilters(modelBuilder);
    }

    private void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // Employee indexes
        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Email)
            .IsUnique()
            .HasDatabaseName("IX_Employees_Email");

        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Department)
            .HasDatabaseName("IX_Employees_Department");

        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Role)
            .HasDatabaseName("IX_Employees_Role");

        // EvaluationResult indexes for performance
        modelBuilder.Entity<EvaluationResult>()
            .HasIndex(e => new { e.SessionId, e.EvaluatorEmployeeId })
            .HasDatabaseName("IX_EvaluationResults_Session_Evaluator");

        modelBuilder.Entity<EvaluationResult>()
            .HasIndex(e => new { e.SessionId, e.EvaluatedEmployeeId })
            .HasDatabaseName("IX_EvaluationResults_Session_Evaluated");

        modelBuilder.Entity<EvaluationResult>()
            .HasIndex(e => new { e.SessionId, e.EvaluatorEmployeeId, e.EvaluatedEmployeeId, e.CompetencyId })
            .IsUnique()
            .HasDatabaseName("IX_EvaluationResults_Unique_Evaluation");

        // AnonymousMapping indexes
        modelBuilder.Entity<AnonymousMapping>()
            .HasIndex(e => e.AnonymousCode)
            .IsUnique()
            .HasDatabaseName("IX_AnonymousMappings_Code");

        modelBuilder.Entity<AnonymousMapping>()
            .HasIndex(e => new { e.SessionId, e.EvaluatorEmployeeId })
            .IsUnique()
            .HasDatabaseName("IX_AnonymousMappings_Session_Evaluator");

        // EvaluationSession indexes
        modelBuilder.Entity<EvaluationSession>()
            .HasIndex(e => new { e.StartDate, e.EndDate })
            .HasDatabaseName("IX_EvaluationSessions_DateRange");
    }

    private void ConfigureGlobalFilters(ModelBuilder modelBuilder)
    {
        // Add any global query filters if needed
        // For example, soft delete filters could be added here
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps before saving
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Employee || e.Entity is Competency || 
                       e.Entity is EvaluationSession || e.Entity is EvaluationResult || 
                       e.Entity is AnonymousMapping);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
            {
                if (entry.Entity.GetType().GetProperty("UpdatedAt") != null)
                {
                    entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
