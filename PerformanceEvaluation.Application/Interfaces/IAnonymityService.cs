using PerformanceEvaluation.Application.DTOs;

namespace PerformanceEvaluation.Application.Interfaces;

public interface IAnonymityService
{
    Task<Guid> GetAnonymousCodeAsync(int sessionId, int evaluatorId);
    Task<int?> GetEvaluatorIdFromAnonymousCodeAsync(Guid anonymousCode);
    Task<AnonymousMappingDto> CreateAnonymousMappingAsync(int sessionId, int evaluatorId);
    Task<IEnumerable<AnonymousMappingDto>> GetAnonymousMappingsForSessionAsync(int sessionId);
    Task<bool> ValidateAnonymousCodeAsync(Guid anonymousCode, int sessionId);
}
