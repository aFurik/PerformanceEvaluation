using PerformanceEvaluation.Application.DTOs;

namespace PerformanceEvaluation.Application.Interfaces;

public interface IReportingService
{
    Task<EmployeeReportDto> GetEmployeeReportAsync(int sessionId, int employeeId);
    Task<SessionSummaryReportDto> GetSessionSummaryReportAsync(int sessionId);
    Task<CompetencyAnalysisReportDto> GetCompetencyAnalysisReportAsync(int sessionId);
    Task<DepartmentReportDto> GetDepartmentReportAsync(int sessionId, string department);
    Task<IEnumerable<EvaluationSummaryDto>> GetEvaluationSummariesAsync(int sessionId);
}
