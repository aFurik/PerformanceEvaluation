using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PerformanceEvaluation.Application.Interfaces;
using PerformanceEvaluation.Application.Services;
using PerformanceEvaluation.Infrastructure.Data;
using PerformanceEvaluation.Infrastructure.Repositories;

namespace PerformanceEvaluation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Context
        services.AddDbContext<PerformanceEvaluationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repository Registration
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<ICompetencyRepository, CompetencyRepository>();
        services.AddScoped<IEvaluationSessionRepository, EvaluationSessionRepository>();
        services.AddScoped<IEvaluationResultRepository, EvaluationResultRepository>();
        services.AddScoped<IAnonymousMappingRepository, AnonymousMappingRepository>();

        // Service Registration
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<ICompetencyService, CompetencyService>();
        services.AddScoped<IEvaluationSessionService, EvaluationSessionService>();
        services.AddScoped<IEvaluationService, EvaluationService>();
        services.AddScoped<IAnonymityService, AnonymityService>();
        services.AddScoped<IReportingService, ReportingService>();

        return services;
    }
}
