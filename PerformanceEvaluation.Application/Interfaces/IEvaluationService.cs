using PerformanceEvaluation.Application.DTOs;

namespace PerformanceEvaluation.Application.Interfaces;

public interface IEvaluationService
{
    Task<IEnumerable<EmployeeAssignmentDto>> GetMyAssignmentsAsync(int sessionId, int evaluatorId);
    Task<EvaluationResultDto> SubmitEvaluationAsync(Guid anonymousCode, SubmitEvaluationDto submitEvaluationDto);
    Task<IEnumerable<EvaluationResultDto>> GetMyEvaluationResultsAsync(int sessionId, int employeeId);
    Task<bool> HasEvaluationBeenSubmittedAsync(int sessionId, int evaluatorId, int evaluatedEmployeeId, int competencyId);
    Task<EvaluationResultDto?> UpdateEvaluationAsync(int evaluationId, UpdateEvaluationDto updateEvaluationDto);
    Task<bool> DeleteEvaluationAsync(int evaluationId);
    Task<IEnumerable<EvaluationResultDto>> GetEvaluationsBySessionAsync(int sessionId);
}
