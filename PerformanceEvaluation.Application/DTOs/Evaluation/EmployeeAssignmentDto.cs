namespace PerformanceEvaluation.Application.DTOs;

public class EmployeeAssignmentDto
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public IEnumerable<CompetencyDto> Competencies { get; set; } = new List<CompetencyDto>();
    public bool HasBeenEvaluated { get; set; }
}
