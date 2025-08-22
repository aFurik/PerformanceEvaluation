namespace PerformanceEvaluation.Application.DTOs;

public class DepartmentReportDto
{
    public string Department { get; set; } = string.Empty;
    public int SessionId { get; set; }
    public string SessionTitle { get; set; } = string.Empty;
    public int TotalEmployees { get; set; }
    public double AverageScore { get; set; }
    public IEnumerable<EmployeeSummaryDto> EmployeeSummaries { get; set; } = new List<EmployeeSummaryDto>();
    public IEnumerable<CompetencyAverageDto> CompetencyAverages { get; set; } = new List<CompetencyAverageDto>();
}

public class EmployeeSummaryDto
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public double AverageScore { get; set; }
    public int EvaluationCount { get; set; }
}

public class EvaluationSummaryDto
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public double AverageScore { get; set; }
    public int CompletedEvaluations { get; set; }
    public int TotalPossibleEvaluations { get; set; }
    public double CompletionPercentage { get; set; }
}
