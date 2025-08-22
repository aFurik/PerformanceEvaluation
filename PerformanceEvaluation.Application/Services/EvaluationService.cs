using AutoMapper;
using PerformanceEvaluation.Application.DTOs;
using PerformanceEvaluation.Application.Interfaces;
using PerformanceEvaluation.Domain.Entities;

namespace PerformanceEvaluation.Application.Services;

public class EvaluationService : IEvaluationService
{
    private readonly IEvaluationResultRepository _evaluationRepository;
    private readonly IAnonymityService _anonymityService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICompetencyRepository _competencyRepository;
    private readonly IEvaluationSessionRepository _sessionRepository;
    private readonly IMapper _mapper;

    public EvaluationService(
        IEvaluationResultRepository evaluationRepository,
        IAnonymityService anonymityService,
        IEmployeeRepository employeeRepository,
        ICompetencyRepository competencyRepository,
        IEvaluationSessionRepository sessionRepository,
        IMapper mapper)
    {
        _evaluationRepository = evaluationRepository;
        _anonymityService = anonymityService;
        _employeeRepository = employeeRepository;
        _competencyRepository = competencyRepository;
        _sessionRepository = sessionRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmployeeAssignmentDto>> GetMyAssignmentsAsync(int sessionId, int evaluatorId)
    {
        // Validate session exists and is active
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null || !session.IsActive)
        {
            throw new InvalidOperationException("Session not found or not active.");
        }

        // Get all employees except the evaluator (no self-evaluation)
        var allEmployees = await _employeeRepository.GetAllAsync();
        var employeesToEvaluate = allEmployees.Where(e => e.Id != evaluatorId).ToList();

        // Get all competencies
        var competencies = await _competencyRepository.GetAllAsync();

        // Get existing evaluations by this evaluator in this session
        var existingEvaluations = await _evaluationRepository.GetBySessionAndEvaluatorAsync(sessionId, evaluatorId);

        var assignments = new List<EmployeeAssignmentDto>();

        foreach (var employee in employeesToEvaluate)
        {
            var hasBeenEvaluated = existingEvaluations.Any(e => e.EvaluatedEmployeeId == employee.Id);
            
            assignments.Add(new EmployeeAssignmentDto
            {
                EmployeeId = employee.Id,
                FullName = employee.FullName,
                Position = employee.Position,
                Department = employee.Department,
                Competencies = _mapper.Map<IEnumerable<CompetencyDto>>(competencies),
                HasBeenEvaluated = hasBeenEvaluated
            });
        }

        return assignments;
    }

    public async Task<EvaluationResultDto> SubmitEvaluationAsync(Guid anonymousCode, SubmitEvaluationDto submitEvaluationDto)
    {
        // Get evaluator ID from anonymous code
        var evaluatorId = await _anonymityService.GetEvaluatorIdFromAnonymousCodeAsync(anonymousCode);
        if (evaluatorId == null)
        {
            throw new InvalidOperationException("Invalid anonymous code.");
        }

        // Validate anonymous code belongs to the session
        var isValidCode = await _anonymityService.ValidateAnonymousCodeAsync(anonymousCode, submitEvaluationDto.SessionId);
        if (!isValidCode)
        {
            throw new InvalidOperationException("Anonymous code does not belong to this session.");
        }

        // Validate session is active
        var session = await _sessionRepository.GetByIdAsync(submitEvaluationDto.SessionId);
        if (session == null || !session.IsActive)
        {
            throw new InvalidOperationException("Session not found or not active.");
        }

        // Prevent self-evaluation
        if (evaluatorId == submitEvaluationDto.EvaluatedEmployeeId)
        {
            throw new InvalidOperationException("Self-evaluation is not allowed.");
        }

        // Check for duplicate evaluation
        var existingEvaluation = await _evaluationRepository.GetBySessionEvaluatorEvaluatedAndCompetencyAsync(
            submitEvaluationDto.SessionId, 
            evaluatorId.Value, 
            submitEvaluationDto.EvaluatedEmployeeId, 
            submitEvaluationDto.CompetencyId);

        if (existingEvaluation != null)
        {
            throw new InvalidOperationException("Evaluation already exists for this employee and competency.");
        }

        // Validate entities exist
        var evaluatedEmployee = await _employeeRepository.GetByIdAsync(submitEvaluationDto.EvaluatedEmployeeId);
        if (evaluatedEmployee == null)
        {
            throw new InvalidOperationException("Evaluated employee not found.");
        }

        var competency = await _competencyRepository.GetByIdAsync(submitEvaluationDto.CompetencyId);
        if (competency == null)
        {
            throw new InvalidOperationException("Competency not found.");
        }

        // Create evaluation
        var evaluation = new EvaluationResult(
            submitEvaluationDto.SessionId,
            submitEvaluationDto.EvaluatedEmployeeId,
            evaluatorId.Value,
            submitEvaluationDto.CompetencyId,
            submitEvaluationDto.Score,
            submitEvaluationDto.Comment
        );

        await _evaluationRepository.AddAsync(evaluation);
        await _evaluationRepository.SaveChangesAsync();

        // Return DTO without exposing evaluator information
        return new EvaluationResultDto
        {
            Id = evaluation.Id,
            SessionId = evaluation.SessionId,
            EvaluatedEmployeeId = evaluation.EvaluatedEmployeeId,
            EvaluatedEmployeeName = evaluatedEmployee.FullName,
            CompetencyId = evaluation.CompetencyId,
            CompetencyName = competency.Name,
            Score = evaluation.Score,
            Comment = evaluation.Comment,
            CreatedAt = evaluation.CreatedAt
        };
    }

    public async Task<IEnumerable<EvaluationResultDto>> GetMyEvaluationResultsAsync(int sessionId, int employeeId)
    {
        var evaluations = await _evaluationRepository.GetBySessionAndEvaluatedEmployeeAsync(sessionId, employeeId);
        
        var results = new List<EvaluationResultDto>();
        
        foreach (var evaluation in evaluations)
        {
            var competency = await _competencyRepository.GetByIdAsync(evaluation.CompetencyId);
            
            results.Add(new EvaluationResultDto
            {
                Id = evaluation.Id,
                SessionId = evaluation.SessionId,
                EvaluatedEmployeeId = evaluation.EvaluatedEmployeeId,
                EvaluatedEmployeeName = "", // Not needed for own results
                CompetencyId = evaluation.CompetencyId,
                CompetencyName = competency?.Name ?? "",
                Score = evaluation.Score,
                Comment = evaluation.Comment,
                CreatedAt = evaluation.CreatedAt
                // Note: EvaluatorEmployeeId is NEVER exposed
            });
        }

        return results;
    }

    public async Task<bool> HasEvaluationBeenSubmittedAsync(int sessionId, int evaluatorId, int evaluatedEmployeeId, int competencyId)
    {
        var evaluation = await _evaluationRepository.GetBySessionEvaluatorEvaluatedAndCompetencyAsync(
            sessionId, evaluatorId, evaluatedEmployeeId, competencyId);
        return evaluation != null;
    }

    public async Task<EvaluationResultDto?> UpdateEvaluationAsync(int evaluationId, UpdateEvaluationDto updateEvaluationDto)
    {
        var evaluation = await _evaluationRepository.GetByIdAsync(evaluationId);
        if (evaluation == null)
        {
            return null;
        }

        evaluation.UpdateScore(updateEvaluationDto.Score, updateEvaluationDto.Comment);
        
        await _evaluationRepository.UpdateAsync(evaluation);
        await _evaluationRepository.SaveChangesAsync();

        var competency = await _competencyRepository.GetByIdAsync(evaluation.CompetencyId);
        var evaluatedEmployee = await _employeeRepository.GetByIdAsync(evaluation.EvaluatedEmployeeId);

        return new EvaluationResultDto
        {
            Id = evaluation.Id,
            SessionId = evaluation.SessionId,
            EvaluatedEmployeeId = evaluation.EvaluatedEmployeeId,
            EvaluatedEmployeeName = evaluatedEmployee?.FullName ?? "",
            CompetencyId = evaluation.CompetencyId,
            CompetencyName = competency?.Name ?? "",
            Score = evaluation.Score,
            Comment = evaluation.Comment,
            CreatedAt = evaluation.CreatedAt
        };
    }

    public async Task<bool> DeleteEvaluationAsync(int evaluationId)
    {
        var evaluation = await _evaluationRepository.GetByIdAsync(evaluationId);
        if (evaluation == null)
        {
            return false;
        }

        await _evaluationRepository.DeleteAsync(evaluation);
        await _evaluationRepository.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<EvaluationResultDto>> GetEvaluationsBySessionAsync(int sessionId)
    {
        var evaluations = await _evaluationRepository.GetBySessionIdAsync(sessionId);
        
        var results = new List<EvaluationResultDto>();
        
        foreach (var evaluation in evaluations)
        {
            var competency = await _competencyRepository.GetByIdAsync(evaluation.CompetencyId);
            var evaluatedEmployee = await _employeeRepository.GetByIdAsync(evaluation.EvaluatedEmployeeId);
            
            results.Add(new EvaluationResultDto
            {
                Id = evaluation.Id,
                SessionId = evaluation.SessionId,
                EvaluatedEmployeeId = evaluation.EvaluatedEmployeeId,
                EvaluatedEmployeeName = evaluatedEmployee?.FullName ?? "",
                CompetencyId = evaluation.CompetencyId,
                CompetencyName = competency?.Name ?? "",
                Score = evaluation.Score,
                Comment = evaluation.Comment,
                CreatedAt = evaluation.CreatedAt
                // Note: EvaluatorEmployeeId is NEVER exposed for anonymity
            });
        }

        return results;
    }
}

// Repository interface for Infrastructure layer
public interface IEvaluationResultRepository
{
    Task<EvaluationResult?> GetByIdAsync(int id);
    Task<IEnumerable<EvaluationResult>> GetBySessionIdAsync(int sessionId);
    Task<IEnumerable<EvaluationResult>> GetBySessionAndEvaluatorAsync(int sessionId, int evaluatorId);
    Task<IEnumerable<EvaluationResult>> GetBySessionAndEvaluatedEmployeeAsync(int sessionId, int evaluatedEmployeeId);
    Task<EvaluationResult?> GetBySessionEvaluatorEvaluatedAndCompetencyAsync(int sessionId, int evaluatorId, int evaluatedEmployeeId, int competencyId);
    Task<EvaluationResult> AddAsync(EvaluationResult evaluation);
    Task<EvaluationResult> UpdateAsync(EvaluationResult evaluation);
    Task DeleteAsync(EvaluationResult evaluation);
    Task SaveChangesAsync();
}

public interface ICompetencyRepository
{
    Task<IEnumerable<Competency>> GetAllAsync();
    Task<Competency?> GetByIdAsync(int id);
    Task<Competency> AddAsync(Competency competency);
    Task<Competency> UpdateAsync(Competency competency);
    Task DeleteAsync(Competency competency);
    Task<bool> ExistsAsync(int id);
    Task SaveChangesAsync();
}
