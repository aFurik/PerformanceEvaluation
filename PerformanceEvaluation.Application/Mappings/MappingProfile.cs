using AutoMapper;
using PerformanceEvaluation.Application.DTOs;
using PerformanceEvaluation.Domain.Entities;

namespace PerformanceEvaluation.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Employee mappings
        CreateMap<Employee, EmployeeDto>();
        CreateMap<CreateEmployeeDto, Employee>();
        CreateMap<UpdateEmployeeDto, Employee>();

        // Competency mappings
        CreateMap<Competency, CompetencyDto>();
        CreateMap<CreateCompetencyDto, Competency>();
        CreateMap<UpdateCompetencyDto, Competency>();

        // EvaluationSession mappings
        CreateMap<EvaluationSession, EvaluationSessionDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
        CreateMap<CreateEvaluationSessionDto, EvaluationSession>();
        CreateMap<UpdateEvaluationSessionDto, EvaluationSession>();

        // EvaluationResult mappings - CRITICAL: Never expose EvaluatorEmployeeId
        CreateMap<EvaluationResult, EvaluationResultDto>()
            .ForMember(dest => dest.EvaluatedEmployeeName, opt => opt.Ignore())
            .ForMember(dest => dest.CompetencyName, opt => opt.Ignore());

        // AnonymousMapping mappings - CRITICAL: Never expose EvaluatorEmployeeId
        CreateMap<AnonymousMapping, AnonymousMappingDto>();

        // Additional mappings for complex scenarios
        CreateMap<Employee, EmployeeAssignmentDto>()
            .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Competencies, opt => opt.Ignore())
            .ForMember(dest => dest.HasBeenEvaluated, opt => opt.Ignore());

        CreateMap<Employee, EmployeeSummaryDto>()
            .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AverageScore, opt => opt.Ignore())
            .ForMember(dest => dest.EvaluationCount, opt => opt.Ignore());
    }
}
