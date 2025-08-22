using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerformanceEvaluation.Domain.Entities;

namespace PerformanceEvaluation.Infrastructure.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Position)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Department)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Role)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Navigation properties
        builder.HasMany(e => e.EvaluationsGiven)
            .WithOne(er => er.EvaluatorEmployee)
            .HasForeignKey(er => er.EvaluatorEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.EvaluationsReceived)
            .WithOne(er => er.EvaluatedEmployee)
            .HasForeignKey(er => er.EvaluatedEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.AnonymousMappings)
            .WithOne(am => am.EvaluatorEmployee)
            .HasForeignKey(am => am.EvaluatorEmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
