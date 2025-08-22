namespace PerformanceEvaluation.Application.DTOs;

public class CompetencyAnalysisReportDto
{
    public int SessionId { get; set; }
    public string SessionTitle { get; set; } = string.Empty;
    public IEnumerable<CompetencyDetailDto> CompetencyDetails { get; set; } = new List<CompetencyDetailDto>();
}

public class CompetencyDetailDto
{
    public int CompetencyId { get; set; }
    public string CompetencyName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double AverageScore { get; set; }
    public int TotalEvaluations { get; set; }
    public IEnumerable<ScoreDistributionDto> ScoreDistribution { get; set; } = new List<ScoreDistributionDto>();
    public IEnumerable<DepartmentCompetencyDto> DepartmentBreakdown { get; set; } = new List<DepartmentCompetencyDto>();
}

public class ScoreDistributionDto
{
    public int Score { get; set; }
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class DepartmentCompetencyDto
{
    public string Department { get; set; } = string.Empty;
    public double AverageScore { get; set; }
    public int EvaluationCount { get; set; }
}
