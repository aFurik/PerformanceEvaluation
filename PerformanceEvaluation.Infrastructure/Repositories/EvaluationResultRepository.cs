using Microsoft.EntityFrameworkCore;
using PerformanceEvaluation.Application.Services;
using PerformanceEvaluation.Domain.Entities;
using PerformanceEvaluation.Infrastructure.Data;

namespace PerformanceEvaluation.Infrastructure.Repositories;

public class EvaluationResultRepository : BaseRepository<EvaluationResult>, IEvaluationResultRepository
{
    public EvaluationResultRepository(PerformanceEvaluationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<EvaluationResult>> GetBySessionIdAsync(int sessionId)
    {
        return await _dbSet
            .Where(er => er.SessionId == sessionId)
            .Include(er => er.Session)
            .Include(er => er.EvaluatedEmployee)
            .Include(er => er.EvaluatorEmployee)
            .Include(er => er.Competency)
            .ToListAsync();
    }

    public async Task<IEnumerable<EvaluationResult>> GetBySessionAndEvaluatorAsync(int sessionId, int evaluatorId)
    {
        return await _dbSet
            .Where(er => er.SessionId == sessionId && er.EvaluatorEmployeeId == evaluatorId)
            .Include(er => er.Session)
            .Include(er => er.EvaluatedEmployee)
            .Include(er => er.Competency)
            .ToListAsync();
    }

    public async Task<IEnumerable<EvaluationResult>> GetBySessionAndEvaluatedEmployeeAsync(int sessionId, int evaluatedEmployeeId)
    {
        return await _dbSet
            .Where(er => er.SessionId == sessionId && er.EvaluatedEmployeeId == evaluatedEmployeeId)
            .Include(er => er.Session)
            .Include(er => er.Competency)
            .ToListAsync();
    }

    public async Task<EvaluationResult?> GetBySessionEvaluatorEvaluatedAndCompetencyAsync(
        int sessionId, int evaluatorId, int evaluatedEmployeeId, int competencyId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(er => 
                er.SessionId == sessionId && 
                er.EvaluatorEmployeeId == evaluatorId && 
                er.EvaluatedEmployeeId == evaluatedEmployeeId && 
                er.CompetencyId == competencyId);
    }
}
