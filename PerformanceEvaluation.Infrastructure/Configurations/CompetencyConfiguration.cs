using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PerformanceEvaluation.Domain.Entities;

namespace PerformanceEvaluation.Infrastructure.Configurations;

public class CompetencyConfiguration : IEntityTypeConfiguration<Competency>
{
    public void Configure(EntityTypeBuilder<Competency> builder)
    {
        builder.ToTable("Competencies");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        // Navigation properties
        builder.HasMany(c => c.EvaluationResults)
            .WithOne(er => er.Competency)
            .HasForeignKey(er => er.CompetencyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class EvaluationSessionConfiguration : IEntityTypeConfiguration<EvaluationSession>
{
    public void Configure(EntityTypeBuilder<EvaluationSession> builder)
    {
        builder.ToTable("EvaluationSessions");

        builder.HasKey(es => es.Id);

        builder.Property(es => es.Id)
            .ValueGeneratedOnAdd();

        builder.Property(es => es.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(es => es.StartDate)
            .IsRequired();

        builder.Property(es => es.EndDate)
            .IsRequired();

        builder.Property(es => es.CreatedBy)
            .IsRequired();

        builder.Property(es => es.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Navigation properties
        builder.HasMany(es => es.EvaluationResults)
            .WithOne(er => er.Session)
            .HasForeignKey(er => er.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(es => es.AnonymousMappings)
            .WithOne(am => am.Session)
            .HasForeignKey(am => am.SessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class EvaluationResultConfiguration : IEntityTypeConfiguration<EvaluationResult>
{
    public void Configure(EntityTypeBuilder<EvaluationResult> builder)
    {
        builder.ToTable("EvaluationResults");

        builder.HasKey(er => er.Id);

        builder.Property(er => er.Id)
            .ValueGeneratedOnAdd();

        builder.Property(er => er.SessionId)
            .IsRequired();

        builder.Property(er => er.EvaluatedEmployeeId)
            .IsRequired();

        builder.Property(er => er.EvaluatorEmployeeId)
            .IsRequired();

        builder.Property(er => er.CompetencyId)
            .IsRequired();

        builder.Property(er => er.Score)
            .IsRequired();

        builder.Property(er => er.Comment)
            .HasMaxLength(2000);

        builder.Property(er => er.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Unique constraint to prevent duplicate evaluations
        builder.HasIndex(er => new { er.SessionId, er.EvaluatorEmployeeId, er.EvaluatedEmployeeId, er.CompetencyId })
            .IsUnique()
            .HasDatabaseName("IX_EvaluationResults_Unique_Evaluation");
    }
}

public class AnonymousMappingConfiguration : IEntityTypeConfiguration<AnonymousMapping>
{
    public void Configure(EntityTypeBuilder<AnonymousMapping> builder)
    {
        builder.ToTable("AnonymousMappings");

        builder.HasKey(am => am.Id);

        builder.Property(am => am.Id)
            .ValueGeneratedOnAdd();

        builder.Property(am => am.SessionId)
            .IsRequired();

        builder.Property(am => am.EvaluatorEmployeeId)
            .IsRequired();

        builder.Property(am => am.AnonymousCode)
            .IsRequired();

        builder.Property(am => am.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Unique constraints for anonymity integrity
        builder.HasIndex(am => am.AnonymousCode)
            .IsUnique()
            .HasDatabaseName("IX_AnonymousMappings_Code");

        builder.HasIndex(am => new { am.SessionId, am.EvaluatorEmployeeId })
            .IsUnique()
            .HasDatabaseName("IX_AnonymousMappings_Session_Evaluator");
    }
}
