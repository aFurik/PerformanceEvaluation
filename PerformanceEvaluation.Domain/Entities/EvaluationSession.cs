using System.ComponentModel.DataAnnotations;

namespace PerformanceEvaluation.Domain.Entities;

public class EvaluationSession
{
    public int Id { get; private set; }
    
    [Required, MaxLength(200)]
    public string Title { get; private set; } = string.Empty;
    
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    // Navigation properties
    private readonly List<EvaluationResult> _evaluationResults = new();
    private readonly List<AnonymousMapping> _anonymousMappings = new();
    
    public IReadOnlyCollection<EvaluationResult> EvaluationResults => _evaluationResults.AsReadOnly();
    public IReadOnlyCollection<AnonymousMapping> AnonymousMappings => _anonymousMappings.AsReadOnly();
    
    // Private constructor for EF Core
    private EvaluationSession() { }
    
    public EvaluationSession(string title, DateTime startDate, DateTime endDate, int createdBy)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        StartDate = startDate;
        EndDate = endDate;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date");
    }
    
    public void UpdateInfo(string title, DateTime startDate, DateTime endDate)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        StartDate = startDate;
        EndDate = endDate;
        
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date");
    }
    
    public bool IsActive => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate;
}
