using PerformanceEvaluation.Application.Services;
using PerformanceEvaluation.Domain.Entities;
using PerformanceEvaluation.Infrastructure.Data;

namespace PerformanceEvaluation.Infrastructure.Repositories;

public class CompetencyRepository : BaseRepository<Competency>, ICompetencyRepository
{
    public CompetencyRepository(PerformanceEvaluationDbContext context) : base(context)
    {
    }
}
