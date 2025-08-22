namespace PerformanceEvaluation.Application.DTOs;

public class SessionSummaryReportDto
{
    public int SessionId { get; set; }
    public string SessionTitle { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalEmployees { get; set; }
    public int TotalEvaluations { get; set; }
    public int CompletedEvaluations { get; set; }
    public double CompletionPercentage { get; set; }
    public double OverallAverageScore { get; set; }
    public IEnumerable<DepartmentSummaryDto> DepartmentSummaries { get; set; } = new List<DepartmentSummaryDto>();
    public IEnumerable<CompetencyAverageDto> CompetencyAverages { get; set; } = new List<CompetencyAverageDto>();
}

public class DepartmentSummaryDto
{
    public string Department { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public double AverageScore { get; set; }
    public int CompletedEvaluations { get; set; }
}

public class CompetencyAverageDto
{
    public int CompetencyId { get; set; }
    public string CompetencyName { get; set; } = string.Empty;
    public double AverageScore { get; set; }
    public int EvaluationCount { get; set; }
}
