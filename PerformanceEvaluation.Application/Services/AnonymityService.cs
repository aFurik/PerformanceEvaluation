using AutoMapper;
using PerformanceEvaluation.Application.DTOs;
using PerformanceEvaluation.Application.Interfaces;
using PerformanceEvaluation.Domain.Entities;

namespace PerformanceEvaluation.Application.Services;

public class AnonymityService : IAnonymityService
{
    private readonly IAnonymousMappingRepository _anonymousMappingRepository;
    private readonly IEvaluationSessionRepository _sessionRepository;
    private readonly IMapper _mapper;

    public AnonymityService(
        IAnonymousMappingRepository anonymousMappingRepository,
        IEvaluationSessionRepository sessionRepository,
        IMapper mapper)
    {
        _anonymousMappingRepository = anonymousMappingRepository;
        _sessionRepository = sessionRepository;
        _mapper = mapper;
    }

    public async Task<Guid> GetAnonymousCodeAsync(int sessionId, int evaluatorId)
    {
        // Check if session exists and is valid
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            throw new InvalidOperationException($"Session with ID {sessionId} not found.");
        }

        // Check if mapping already exists
        var existingMapping = await _anonymousMappingRepository.GetBySessionAndEvaluatorAsync(sessionId, evaluatorId);
        if (existingMapping != null)
        {
            return existingMapping.AnonymousCode;
        }

        // Create new anonymous mapping
        var mapping = new AnonymousMapping(sessionId, evaluatorId);
        await _anonymousMappingRepository.AddAsync(mapping);
        await _anonymousMappingRepository.SaveChangesAsync();

        return mapping.AnonymousCode;
    }

    public async Task<int?> GetEvaluatorIdFromAnonymousCodeAsync(Guid anonymousCode)
    {
        var mapping = await _anonymousMappingRepository.GetByAnonymousCodeAsync(anonymousCode);
        return mapping?.EvaluatorEmployeeId;
    }

    public async Task<AnonymousMappingDto> CreateAnonymousMappingAsync(int sessionId, int evaluatorId)
    {
        // Check if session exists
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            throw new InvalidOperationException($"Session with ID {sessionId} not found.");
        }

        // Check if mapping already exists
        var existingMapping = await _anonymousMappingRepository.GetBySessionAndEvaluatorAsync(sessionId, evaluatorId);
        if (existingMapping != null)
        {
            return _mapper.Map<AnonymousMappingDto>(existingMapping);
        }

        var mapping = new AnonymousMapping(sessionId, evaluatorId);
        await _anonymousMappingRepository.AddAsync(mapping);
        await _anonymousMappingRepository.SaveChangesAsync();

        return _mapper.Map<AnonymousMappingDto>(mapping);
    }

    public async Task<IEnumerable<AnonymousMappingDto>> GetAnonymousMappingsForSessionAsync(int sessionId)
    {
        var mappings = await _anonymousMappingRepository.GetBySessionIdAsync(sessionId);
        return _mapper.Map<IEnumerable<AnonymousMappingDto>>(mappings);
    }

    public async Task<bool> ValidateAnonymousCodeAsync(Guid anonymousCode, int sessionId)
    {
        var mapping = await _anonymousMappingRepository.GetByAnonymousCodeAsync(anonymousCode);
        return mapping != null && mapping.SessionId == sessionId;
    }
}

// Repository interfaces for Infrastructure layer
public interface IAnonymousMappingRepository
{
    Task<AnonymousMapping?> GetByAnonymousCodeAsync(Guid anonymousCode);
    Task<AnonymousMapping?> GetBySessionAndEvaluatorAsync(int sessionId, int evaluatorId);
    Task<IEnumerable<AnonymousMapping>> GetBySessionIdAsync(int sessionId);
    Task<AnonymousMapping> AddAsync(AnonymousMapping mapping);
    Task SaveChangesAsync();
}

public interface IEvaluationSessionRepository
{
    Task<EvaluationSession?> GetByIdAsync(int id);
    Task<IEnumerable<EvaluationSession>> GetAllAsync();
    Task<IEnumerable<EvaluationSession>> GetActiveSessionsAsync();
    Task<EvaluationSession> AddAsync(EvaluationSession session);
    Task<EvaluationSession> UpdateAsync(EvaluationSession session);
    Task DeleteAsync(EvaluationSession session);
    Task<bool> ExistsAsync(int id);
    Task SaveChangesAsync();
}
