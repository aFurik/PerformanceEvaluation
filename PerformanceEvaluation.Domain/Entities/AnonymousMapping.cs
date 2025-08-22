namespace PerformanceEvaluation.Domain.Entities;

public class AnonymousMapping
{
    public int Id { get; private set; }
    public int SessionId { get; private set; }
    public int EvaluatorEmployeeId { get; private set; }
    public Guid AnonymousCode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    // Navigation properties
    public EvaluationSession Session { get; private set; } = null!;
    public Employee EvaluatorEmployee { get; private set; } = null!;
    
    // Private constructor for EF Core
    private AnonymousMapping() { }
    
    public AnonymousMapping(int sessionId, int evaluatorEmployeeId)
    {
        SessionId = sessionId;
        EvaluatorEmployeeId = evaluatorEmployeeId;
        AnonymousCode = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}
