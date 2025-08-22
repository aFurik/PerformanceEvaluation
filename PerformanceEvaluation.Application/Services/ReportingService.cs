using AutoMapper;
using PerformanceEvaluation.Application.DTOs;
using PerformanceEvaluation.Application.Interfaces;

namespace PerformanceEvaluation.Application.Services;

public class ReportingService : IReportingService
{
    private readonly IEvaluationResultRepository _evaluationRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICompetencyRepository _competencyRepository;
    private readonly IEvaluationSessionRepository _sessionRepository;
    private readonly IMapper _mapper;

    public ReportingService(
        IEvaluationResultRepository evaluationRepository,
        IEmployeeRepository employeeRepository,
        ICompetencyRepository competencyRepository,
        IEvaluationSessionRepository sessionRepository,
        IMapper mapper)
    {
        _evaluationRepository = evaluationRepository;
        _employeeRepository = employeeRepository;
        _competencyRepository = competencyRepository;
        _sessionRepository = sessionRepository;
        _mapper = mapper;
    }

    public async Task<EmployeeReportDto> GetEmployeeReportAsync(int sessionId, int employeeId)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null)
        {
            throw new InvalidOperationException($"Employee with ID {employeeId} not found.");
        }

        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            throw new InvalidOperationException($"Session with ID {sessionId} not found.");
        }

        var evaluations = await _evaluationRepository.GetBySessionAndEvaluatedEmployeeAsync(sessionId, employeeId);
        var competencies = await _competencyRepository.GetAllAsync();

        var competencyScores = new List<CompetencyScoreDto>();
        var allComments = new List<string>();

        foreach (var competency in competencies)
        {
            var competencyEvaluations = evaluations.Where(e => e.CompetencyId == competency.Id).ToList();
            
            if (competencyEvaluations.Any())
            {
                var averageScore = competencyEvaluations.Average(e => e.Score);
                var comments = competencyEvaluations
                    .Where(e => !string.IsNullOrWhiteSpace(e.Comment))
                    .Select(e => e.Comment)
                    .ToList();

                competencyScores.Add(new CompetencyScoreDto
                {
                    CompetencyId = competency.Id,
                    CompetencyName = competency.Name,
                    AverageScore = Math.Round(averageScore, 2),
                    EvaluationCount = competencyEvaluations.Count,
                    Comments = comments
                });

                allComments.AddRange(comments);
            }
        }

        var overallAverage = competencyScores.Any() ? 
            Math.Round(competencyScores.Average(cs => cs.AverageScore), 2) : 0;

        return new EmployeeReportDto
        {
            EmployeeId = employee.Id,
            FullName = employee.FullName,
            Position = employee.Position,
            Department = employee.Department,
            SessionId = session.Id,
            SessionTitle = session.Title,
            CompetencyScores = competencyScores,
            OverallAverageScore = overallAverage,
            TotalEvaluations = evaluations.Count(),
            AnonymousComments = allComments // All comments are anonymous
        };
    }

    public async Task<SessionSummaryReportDto> GetSessionSummaryReportAsync(int sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            throw new InvalidOperationException($"Session with ID {sessionId} not found.");
        }

        var allEmployees = await _employeeRepository.GetAllAsync();
        var allEvaluations = await _evaluationRepository.GetBySessionIdAsync(sessionId);
        var competencies = await _competencyRepository.GetAllAsync();

        // Department summaries
        var departmentSummaries = allEmployees
            .GroupBy(e => e.Department)
            .Select(g => new DepartmentSummaryDto
            {
                Department = g.Key,
                EmployeeCount = g.Count(),
                AverageScore = CalculateDepartmentAverageScore(g.Select(e => e.Id), allEvaluations),
                CompletedEvaluations = allEvaluations.Count(e => g.Any(emp => emp.Id == e.EvaluatedEmployeeId))
            }).ToList();

        // Competency averages
        var competencyAverages = competencies.Select(c => new CompetencyAverageDto
        {
            CompetencyId = c.Id,
            CompetencyName = c.Name,
            AverageScore = CalculateCompetencyAverageScore(c.Id, allEvaluations),
            EvaluationCount = allEvaluations.Count(e => e.CompetencyId == c.Id)
        }).ToList();

        var totalPossibleEvaluations = allEmployees.Count() * competencies.Count() * (allEmployees.Count() - 1); // Excluding self-evaluations
        var completionPercentage = totalPossibleEvaluations > 0 ? 
            Math.Round((double)allEvaluations.Count() / totalPossibleEvaluations * 100, 2) : 0;

        return new SessionSummaryReportDto
        {
            SessionId = session.Id,
            SessionTitle = session.Title,
            StartDate = session.StartDate,
            EndDate = session.EndDate,
            TotalEmployees = allEmployees.Count(),
            TotalEvaluations = allEvaluations.Count(),
            CompletedEvaluations = allEvaluations.Count(),
            CompletionPercentage = completionPercentage,
            OverallAverageScore = allEvaluations.Any() ? Math.Round(allEvaluations.Average(e => e.Score), 2) : 0,
            DepartmentSummaries = departmentSummaries,
            CompetencyAverages = competencyAverages
        };
    }

    public async Task<CompetencyAnalysisReportDto> GetCompetencyAnalysisReportAsync(int sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            throw new InvalidOperationException($"Session with ID {sessionId} not found.");
        }

        var allEvaluations = await _evaluationRepository.GetBySessionIdAsync(sessionId);
        var competencies = await _competencyRepository.GetAllAsync();
        var allEmployees = await _employeeRepository.GetAllAsync();

        var competencyDetails = new List<CompetencyDetailDto>();

        foreach (var competency in competencies)
        {
            var competencyEvaluations = allEvaluations.Where(e => e.CompetencyId == competency.Id).ToList();
            
            if (competencyEvaluations.Any())
            {
                // Score distribution
                var scoreDistribution = Enumerable.Range(1, 5)
                    .Select(score => new ScoreDistributionDto
                    {
                        Score = score,
                        Count = competencyEvaluations.Count(e => e.Score == score),
                        Percentage = Math.Round((double)competencyEvaluations.Count(e => e.Score == score) / competencyEvaluations.Count * 100, 2)
                    }).ToList();

                // Department breakdown
                var departmentBreakdown = allEmployees
                    .GroupBy(e => e.Department)
                    .Select(g => new DepartmentCompetencyDto
                    {
                        Department = g.Key,
                        AverageScore = CalculateDepartmentCompetencyScore(g.Select(e => e.Id), competency.Id, allEvaluations),
                        EvaluationCount = competencyEvaluations.Count(e => g.Any(emp => emp.Id == e.EvaluatedEmployeeId))
                    })
                    .Where(d => d.EvaluationCount > 0)
                    .ToList();

                competencyDetails.Add(new CompetencyDetailDto
                {
                    CompetencyId = competency.Id,
                    CompetencyName = competency.Name,
                    Description = competency.Description,
                    AverageScore = Math.Round(competencyEvaluations.Average(e => e.Score), 2),
                    TotalEvaluations = competencyEvaluations.Count,
                    ScoreDistribution = scoreDistribution,
                    DepartmentBreakdown = departmentBreakdown
                });
            }
        }

        return new CompetencyAnalysisReportDto
        {
            SessionId = session.Id,
            SessionTitle = session.Title,
            CompetencyDetails = competencyDetails
        };
    }

    public async Task<DepartmentReportDto> GetDepartmentReportAsync(int sessionId, string department)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            throw new InvalidOperationException($"Session with ID {sessionId} not found.");
        }

        var departmentEmployees = await _employeeRepository.GetByDepartmentAsync(department);
        var allEvaluations = await _evaluationRepository.GetBySessionIdAsync(sessionId);
        var competencies = await _competencyRepository.GetAllAsync();

        var departmentEvaluations = allEvaluations
            .Where(e => departmentEmployees.Any(emp => emp.Id == e.EvaluatedEmployeeId))
            .ToList();

        var employeeSummaries = departmentEmployees.Select(emp =>
        {
            var empEvaluations = departmentEvaluations.Where(e => e.EvaluatedEmployeeId == emp.Id).ToList();
            return new EmployeeSummaryDto
            {
                EmployeeId = emp.Id,
                FullName = emp.FullName,
                Position = emp.Position,
                AverageScore = empEvaluations.Any() ? Math.Round(empEvaluations.Average(e => e.Score), 2) : 0,
                EvaluationCount = empEvaluations.Count
            };
        }).ToList();

        var competencyAverages = competencies.Select(c => new CompetencyAverageDto
        {
            CompetencyId = c.Id,
            CompetencyName = c.Name,
            AverageScore = CalculateDepartmentCompetencyScore(departmentEmployees.Select(e => e.Id), c.Id, allEvaluations),
            EvaluationCount = departmentEvaluations.Count(e => e.CompetencyId == c.Id)
        }).Where(ca => ca.EvaluationCount > 0).ToList();

        return new DepartmentReportDto
        {
            Department = department,
            SessionId = session.Id,
            SessionTitle = session.Title,
            TotalEmployees = departmentEmployees.Count(),
            AverageScore = departmentEvaluations.Any() ? Math.Round(departmentEvaluations.Average(e => e.Score), 2) : 0,
            EmployeeSummaries = employeeSummaries,
            CompetencyAverages = competencyAverages
        };
    }

    public async Task<IEnumerable<EvaluationSummaryDto>> GetEvaluationSummariesAsync(int sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            throw new InvalidOperationException($"Session with ID {sessionId} not found.");
        }

        var allEmployees = await _employeeRepository.GetAllAsync();
        var allEvaluations = await _evaluationRepository.GetBySessionIdAsync(sessionId);
        var competencies = await _competencyRepository.GetAllAsync();

        var summaries = allEmployees.Select(emp =>
        {
            var empEvaluations = allEvaluations.Where(e => e.EvaluatedEmployeeId == emp.Id).ToList();
            var totalPossible = competencies.Count() * (allEmployees.Count() - 1); // Excluding self-evaluation

            return new EvaluationSummaryDto
            {
                EmployeeId = emp.Id,
                FullName = emp.FullName,
                Department = emp.Department,
                Position = emp.Position,
                AverageScore = empEvaluations.Any() ? Math.Round(empEvaluations.Average(e => e.Score), 2) : 0,
                CompletedEvaluations = empEvaluations.Count,
                TotalPossibleEvaluations = totalPossible,
                CompletionPercentage = totalPossible > 0 ? Math.Round((double)empEvaluations.Count / totalPossible * 100, 2) : 0
            };
        }).ToList();

        return summaries;
    }

    private double CalculateDepartmentAverageScore(IEnumerable<int> employeeIds, IEnumerable<Domain.Entities.EvaluationResult> evaluations)
    {
        var departmentEvaluations = evaluations.Where(e => employeeIds.Contains(e.EvaluatedEmployeeId)).ToList();
        return departmentEvaluations.Any() ? Math.Round(departmentEvaluations.Average(e => e.Score), 2) : 0;
    }

    private double CalculateCompetencyAverageScore(int competencyId, IEnumerable<Domain.Entities.EvaluationResult> evaluations)
    {
        var competencyEvaluations = evaluations.Where(e => e.CompetencyId == competencyId).ToList();
        return competencyEvaluations.Any() ? Math.Round(competencyEvaluations.Average(e => e.Score), 2) : 0;
    }

    private double CalculateDepartmentCompetencyScore(IEnumerable<int> employeeIds, int competencyId, IEnumerable<Domain.Entities.EvaluationResult> evaluations)
    {
        var filteredEvaluations = evaluations
            .Where(e => employeeIds.Contains(e.EvaluatedEmployeeId) && e.CompetencyId == competencyId)
            .ToList();
        return filteredEvaluations.Any() ? Math.Round(filteredEvaluations.Average(e => e.Score), 2) : 0;
    }
}
