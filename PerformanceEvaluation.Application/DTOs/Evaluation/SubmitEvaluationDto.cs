using System.ComponentModel.DataAnnotations;

namespace PerformanceEvaluation.Application.DTOs;

public class SubmitEvaluationDto
{
    [Required]
    public int SessionId { get; set; }
    
    [Required]
    public int EvaluatedEmployeeId { get; set; }
    
    [Required]
    public int CompetencyId { get; set; }
    
    [Required, Range(1, 5)]
    public int Score { get; set; }
    
    [MaxLength(2000)]
    public string Comment { get; set; } = string.Empty;
}
