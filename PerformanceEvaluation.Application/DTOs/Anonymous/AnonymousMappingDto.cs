namespace PerformanceEvaluation.Application.DTOs;

public class AnonymousMappingDto
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public Guid AnonymousCode { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Note: EvaluatorEmployeeId is NEVER exposed for anonymity
}
