using PerformanceEvaluation.Application.DTOs;

namespace PerformanceEvaluation.Application.Interfaces;

public interface ICompetencyService
{
    Task<IEnumerable<CompetencyDto>> GetAllCompetenciesAsync();
    Task<CompetencyDto?> GetCompetencyByIdAsync(int id);
    Task<CompetencyDto> CreateCompetencyAsync(CreateCompetencyDto createCompetencyDto);
    Task<CompetencyDto?> UpdateCompetencyAsync(int id, UpdateCompetencyDto updateCompetencyDto);
    Task<bool> DeleteCompetencyAsync(int id);
    Task<bool> CompetencyExistsAsync(int id);
}
