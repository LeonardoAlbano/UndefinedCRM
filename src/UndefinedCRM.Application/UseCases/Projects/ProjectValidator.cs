using FluentValidation;
using UndefinedCRM.Communication.Requests;

namespace UndefinedCRM.Application.UseCases.Projects;

public class ProjectValidator : AbstractValidator<RequestProjectJson>
{
    public ProjectValidator()
    {
        RuleFor(p => p.ClientId)
            .GreaterThan(0).WithMessage("Client is required");
            
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Project name is required");
            
        When(p => p.StartDate.HasValue && p.EndDate.HasValue, () => {
            RuleFor(p => p.EndDate)
                .GreaterThanOrEqualTo(p => p.StartDate!.Value)
                .WithMessage("End date must be after or equal to start date");
        });
    }
}