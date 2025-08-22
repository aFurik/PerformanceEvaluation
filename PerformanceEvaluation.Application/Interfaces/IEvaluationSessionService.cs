using PerformanceEvaluation.Application.DTOs;

namespace PerformanceEvaluation.Application.Interfaces;

public interface IEvaluationSessionService
{
    Task<IEnumerable<EvaluationSessionDto>> GetAllSessionsAsync();
    Task<EvaluationSessionDto?> GetSessionByIdAsync(int id);
    Task<IEnumerable<EvaluationSessionDto>> GetActiveSessionsAsync();
    Task<EvaluationSessionDto> CreateSessionAsync(CreateEvaluationSessionDto createSessionDto);
    Task<EvaluationSessionDto?> UpdateSessionAsync(int id, UpdateEvaluationSessionDto updateSessionDto);
    Task<bool> DeleteSessionAsync(int id);
    Task<bool> SessionExistsAsync(int id);
    Task<bool> IsSessionActiveAsync(int id);
}
