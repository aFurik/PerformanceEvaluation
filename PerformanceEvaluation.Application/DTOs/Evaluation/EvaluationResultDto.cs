namespace PerformanceEvaluation.Application.DTOs;

public class EvaluationResultDto
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public int EvaluatedEmployeeId { get; set; }
    public string EvaluatedEmployeeName { get; set; } = string.Empty;
    public int CompetencyId { get; set; }
    public string CompetencyName { get; set; } = string.Empty;
    public int Score { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // Note: EvaluatorEmployeeId is NEVER exposed in DTOs for anonymity
}
