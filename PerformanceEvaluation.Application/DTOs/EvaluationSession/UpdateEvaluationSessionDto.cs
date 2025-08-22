using System.ComponentModel.DataAnnotations;

namespace PerformanceEvaluation.Application.DTOs;

public class UpdateEvaluationSessionDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
}
