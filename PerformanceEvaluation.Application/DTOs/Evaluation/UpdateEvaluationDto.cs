using System.ComponentModel.DataAnnotations;

namespace PerformanceEvaluation.Application.DTOs;

public class UpdateEvaluationDto
{
    [Required, Range(1, 5)]
    public int Score { get; set; }
    
    [MaxLength(2000)]
    public string Comment { get; set; } = string.Empty;
}
