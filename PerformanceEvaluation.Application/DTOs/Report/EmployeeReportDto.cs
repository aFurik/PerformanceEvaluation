namespace PerformanceEvaluation.Application.DTOs;

public class EmployeeReportDto
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int SessionId { get; set; }
    public string SessionTitle { get; set; } = string.Empty;
    public IEnumerable<CompetencyScoreDto> CompetencyScores { get; set; } = new List<CompetencyScoreDto>();
    public double OverallAverageScore { get; set; }
    public int TotalEvaluations { get; set; }
    public IEnumerable<string> AnonymousComments { get; set; } = new List<string>();
}

public class CompetencyScoreDto
{
    public int CompetencyId { get; set; }
    public string CompetencyName { get; set; } = string.Empty;
    public double AverageScore { get; set; }
    public int EvaluationCount { get; set; }
    public IEnumerable<string> Comments { get; set; } = new List<string>();
}
