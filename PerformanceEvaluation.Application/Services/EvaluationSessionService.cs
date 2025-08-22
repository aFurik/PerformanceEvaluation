using AutoMapper;
using PerformanceEvaluation.Application.DTOs;
using PerformanceEvaluation.Application.Interfaces;
using PerformanceEvaluation.Domain.Entities;

namespace PerformanceEvaluation.Application.Services;

public class EvaluationSessionService : IEvaluationSessionService
{
    private readonly IEvaluationSessionRepository _sessionRepository;
    private readonly IMapper _mapper;

    public EvaluationSessionService(IEvaluationSessionRepository sessionRepository, IMapper mapper)
    {
        _sessionRepository = sessionRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EvaluationSessionDto>> GetAllSessionsAsync()
    {
        var sessions = await _sessionRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<EvaluationSessionDto>>(sessions);
    }

    public async Task<EvaluationSessionDto?> GetSessionByIdAsync(int id)
    {
        var session = await _sessionRepository.GetByIdAsync(id);
        return session != null ? _mapper.Map<EvaluationSessionDto>(session) : null;
    }

    public async Task<IEnumerable<EvaluationSessionDto>> GetActiveSessionsAsync()
    {
        var sessions = await _sessionRepository.GetActiveSessionsAsync();
        return _mapper.Map<IEnumerable<EvaluationSessionDto>>(sessions);
    }

    public async Task<EvaluationSessionDto> CreateSessionAsync(CreateEvaluationSessionDto createSessionDto)
    {
        // Validate date range
        if (createSessionDto.EndDate <= createSessionDto.StartDate)
        {
            throw new ArgumentException("End date must be after start date.");
        }

        // For now, we'll use a placeholder for CreatedBy - this should come from the current user context
        var session = new EvaluationSession(
            createSessionDto.Title,
            createSessionDto.StartDate,
            createSessionDto.EndDate,
            1 // This should be replaced with actual current user ID from authentication context
        );

        await _sessionRepository.AddAsync(session);
        await _sessionRepository.SaveChangesAsync();

        return _mapper.Map<EvaluationSessionDto>(session);
    }

    public async Task<EvaluationSessionDto?> UpdateSessionAsync(int id, UpdateEvaluationSessionDto updateSessionDto)
    {
        var session = await _sessionRepository.GetByIdAsync(id);
        if (session == null)
        {
            return null;
        }

        session.UpdateInfo(updateSessionDto.Title, updateSessionDto.StartDate, updateSessionDto.EndDate);
        
        await _sessionRepository.UpdateAsync(session);
        await _sessionRepository.SaveChangesAsync();

        return _mapper.Map<EvaluationSessionDto>(session);
    }

    public async Task<bool> DeleteSessionAsync(int id)
    {
        var session = await _sessionRepository.GetByIdAsync(id);
        if (session == null)
        {
            return false;
        }

        await _sessionRepository.DeleteAsync(session);
        await _sessionRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SessionExistsAsync(int id)
    {
        return await _sessionRepository.ExistsAsync(id);
    }

    public async Task<bool> IsSessionActiveAsync(int id)
    {
        var session = await _sessionRepository.GetByIdAsync(id);
        return session?.IsActive ?? false;
    }
}
