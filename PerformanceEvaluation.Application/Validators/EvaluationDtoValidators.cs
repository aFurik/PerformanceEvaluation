using FluentValidation;
using PerformanceEvaluation.Application.DTOs;

namespace PerformanceEvaluation.Application.Validators;

public class SubmitEvaluationDtoValidator : AbstractValidator<SubmitEvaluationDto>
{
    public SubmitEvaluationDtoValidator()
    {
        RuleFor(x => x.SessionId)
            .GreaterThan(0).WithMessage("Session ID must be greater than 0.");

        RuleFor(x => x.EvaluatedEmployeeId)
            .GreaterThan(0).WithMessage("Evaluated Employee ID must be greater than 0.");

        RuleFor(x => x.CompetencyId)
            .GreaterThan(0).WithMessage("Competency ID must be greater than 0.");

        RuleFor(x => x.Score)
            .InclusiveBetween(1, 5).WithMessage("Score must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(2000).WithMessage("Comment cannot exceed 2000 characters.");
    }
}

public class UpdateEvaluationDtoValidator : AbstractValidator<UpdateEvaluationDto>
{
    public UpdateEvaluationDtoValidator()
    {
        RuleFor(x => x.Score)
            .InclusiveBetween(1, 5).WithMessage("Score must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .MaximumLength(2000).WithMessage("Comment cannot exceed 2000 characters.");
    }
}

public class CreateCompetencyDtoValidator : AbstractValidator<CreateCompetencyDto>
{
    public CreateCompetencyDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Competency name is required.")
            .MaximumLength(200).WithMessage("Competency name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");
    }
}

public class UpdateCompetencyDtoValidator : AbstractValidator<UpdateCompetencyDto>
{
    public UpdateCompetencyDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Competency name is required.")
            .MaximumLength(200).WithMessage("Competency name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");
    }
}

public class CreateEvaluationSessionDtoValidator : AbstractValidator<CreateEvaluationSessionDto>
{
    public CreateEvaluationSessionDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Session title is required.")
            .MaximumLength(200).WithMessage("Session title cannot exceed 200 characters.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .LessThan(x => x.EndDate).WithMessage("Start date must be before end date.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");
    }
}

public class UpdateEvaluationSessionDtoValidator : AbstractValidator<UpdateEvaluationSessionDto>
{
    public UpdateEvaluationSessionDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Session title is required.")
            .MaximumLength(200).WithMessage("Session title cannot exceed 200 characters.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .LessThan(x => x.EndDate).WithMessage("Start date must be before end date.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");
    }
}
