using AutoMapper;
using PerformanceEvaluation.Application.DTOs;
using PerformanceEvaluation.Application.Interfaces;
using PerformanceEvaluation.Domain.Entities;

namespace PerformanceEvaluation.Application.Services;

public class CompetencyService : ICompetencyService
{
    private readonly ICompetencyRepository _competencyRepository;
    private readonly IMapper _mapper;

    public CompetencyService(ICompetencyRepository competencyRepository, IMapper mapper)
    {
        _competencyRepository = competencyRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CompetencyDto>> GetAllCompetenciesAsync()
    {
        var competencies = await _competencyRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CompetencyDto>>(competencies);
    }

    public async Task<CompetencyDto?> GetCompetencyByIdAsync(int id)
    {
        var competency = await _competencyRepository.GetByIdAsync(id);
        return competency != null ? _mapper.Map<CompetencyDto>(competency) : null;
    }

    public async Task<CompetencyDto> CreateCompetencyAsync(CreateCompetencyDto createCompetencyDto)
    {
        var competency = new Competency(createCompetencyDto.Name, createCompetencyDto.Description);
        
        await _competencyRepository.AddAsync(competency);
        await _competencyRepository.SaveChangesAsync();

        return _mapper.Map<CompetencyDto>(competency);
    }

    public async Task<CompetencyDto?> UpdateCompetencyAsync(int id, UpdateCompetencyDto updateCompetencyDto)
    {
        var competency = await _competencyRepository.GetByIdAsync(id);
        if (competency == null)
        {
            return null;
        }

        competency.UpdateInfo(updateCompetencyDto.Name, updateCompetencyDto.Description);
        
        await _competencyRepository.UpdateAsync(competency);
        await _competencyRepository.SaveChangesAsync();

        return _mapper.Map<CompetencyDto>(competency);
    }

    public async Task<bool> DeleteCompetencyAsync(int id)
    {
        var competency = await _competencyRepository.GetByIdAsync(id);
        if (competency == null)
        {
            return false;
        }

        await _competencyRepository.DeleteAsync(competency);
        await _competencyRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CompetencyExistsAsync(int id)
    {
        return await _competencyRepository.ExistsAsync(id);
    }
}
