using System.ComponentModel.DataAnnotations;

namespace PerformanceEvaluation.Domain.Entities;

public class EvaluationResult
{
    public int Id { get; private set; }
    public int SessionId { get; private set; }
    public int EvaluatedEmployeeId { get; private set; }
    public int EvaluatorEmployeeId { get; private set; }
    public int CompetencyId { get; private set; }
    
    [Range(1, 5)]
    public int Score { get; private set; }
    
    [MaxLength(2000)]
    public string Comment { get; private set; } = string.Empty;
    
    public DateTime CreatedAt { get; private set; }
    
    // Navigation properties
    public EvaluationSession Session { get; private set; } = null!;
    public Employee EvaluatedEmployee { get; private set; } = null!;
    public Employee EvaluatorEmployee { get; private set; } = null!;
    public Competency Competency { get; private set; } = null!;
    
    // Private constructor for EF Core
    private EvaluationResult() { }
    
    public EvaluationResult(int sessionId, int evaluatedEmployeeId, int evaluatorEmployeeId, 
        int competencyId, int score, string comment = "")
    {
        SessionId = sessionId;
        EvaluatedEmployeeId = evaluatedEmployeeId;
        EvaluatorEmployeeId = evaluatorEmployeeId;
        CompetencyId = competencyId;
        Score = score;
        Comment = comment ?? string.Empty;
        CreatedAt = DateTime.UtcNow;
        
        if (score < 1 || score > 5)
            throw new ArgumentException("Score must be between 1 and 5");
    }
    
    public void UpdateScore(int score, string comment = "")
    {
        if (score < 1 || score > 5)
            throw new ArgumentException("Score must be between 1 and 5");
            
        Score = score;
        Comment = comment ?? string.Empty;
    }
}
