using Microsoft.EntityFrameworkCore;
using PerformanceEvaluation.Application.Services;
using PerformanceEvaluation.Domain.Entities;
using PerformanceEvaluation.Infrastructure.Data;

namespace PerformanceEvaluation.Infrastructure.Repositories;

public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(PerformanceEvaluationDbContext context) : base(context)
    {
    }

    public async Task<Employee?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Email == email);
    }

    public async Task<IEnumerable<Employee>> GetByDepartmentAsync(string department)
    {
        return await _dbSet.Where(e => e.Department == department).ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetByRoleAsync(string role)
    {
        return await _dbSet.Where(e => e.Role == role).ToListAsync();
    }
}
