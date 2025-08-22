using Microsoft.EntityFrameworkCore;
using PerformanceEvaluation.Application.Services;
using PerformanceEvaluation.Domain.Entities;
using PerformanceEvaluation.Infrastructure.Data;

namespace PerformanceEvaluation.Infrastructure.Repositories;

public class AnonymousMappingRepository : BaseRepository<AnonymousMapping>, IAnonymousMappingRepository
{
    public AnonymousMappingRepository(PerformanceEvaluationDbContext context) : base(context)
    {
    }

    public async Task<AnonymousMapping?> GetByAnonymousCodeAsync(Guid anonymousCode)
    {
        return await _dbSet
            .Include(am => am.Session)
            .Include(am => am.EvaluatorEmployee)
            .FirstOrDefaultAsync(am => am.AnonymousCode == anonymousCode);
    }

    public async Task<AnonymousMapping?> GetBySessionAndEvaluatorAsync(int sessionId, int evaluatorId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(am => am.SessionId == sessionId && am.EvaluatorEmployeeId == evaluatorId);
    }

    public async Task<IEnumerable<AnonymousMapping>> GetBySessionIdAsync(int sessionId)
    {
        return await _dbSet
            .Where(am => am.SessionId == sessionId)
            .Include(am => am.Session)
            .ToListAsync();
    }
}
