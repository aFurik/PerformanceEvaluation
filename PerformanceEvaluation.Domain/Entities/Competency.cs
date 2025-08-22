using System.ComponentModel.DataAnnotations;

namespace PerformanceEvaluation.Domain.Entities;

public class Competency
{
    public int Id { get; private set; }
    
    [Required, MaxLength(200)]
    public string Name { get; private set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; private set; } = string.Empty;
    
    [Range(1, 100)]
    public int Weight { get; private set; } = 20;
    
    // Navigation properties
    private readonly List<EvaluationResult> _evaluationResults = new();
    public IReadOnlyCollection<EvaluationResult> EvaluationResults => _evaluationResults.AsReadOnly();
    
    // Private constructor for EF Core
    private Competency() { }
    
    public Competency(string name, string description = "", int weight = 20)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        Weight = weight;
        
        if (weight < 1 || weight > 100)
            throw new ArgumentException("Weight must be between 1 and 100");
    }
    
    public void UpdateInfo(string name, string description = "", int weight = 20)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        Weight = weight;
        
        if (weight < 1 || weight > 100)
            throw new ArgumentException("Weight must be between 1 and 100");
    }
}
