using Microsoft.EntityFrameworkCore;
using PerformanceEvaluation.Application.Services;
using PerformanceEvaluation.Domain.Entities;
using PerformanceEvaluation.Infrastructure.Data;

namespace PerformanceEvaluation.Infrastructure.Repositories;

public class EvaluationSessionRepository : BaseRepository<EvaluationSession>, IEvaluationSessionRepository
{
    public EvaluationSessionRepository(PerformanceEvaluationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<EvaluationSession>> GetActiveSessionsAsync()
    {
        var currentDate = DateTime.UtcNow;
        return await _dbSet
            .Where(es => es.StartDate <= currentDate && es.EndDate >= currentDate)
            .ToListAsync();
    }
}
