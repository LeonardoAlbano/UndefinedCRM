using FluentValidation;
using UndefinedCRM.Communication.Requests;

namespace UndefinedCRM.Application.UseCases.Clients;

public class ClientValidator : AbstractValidator<RequestClientJson>
{
    public ClientValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name is required");
            
        RuleFor(c => c.Surname)
            .NotEmpty().WithMessage("Surname is required");
            
        When(c => !string.IsNullOrEmpty(c.Email), () => {
            RuleFor(c => c.Email)
                .EmailAddress().WithMessage("Invalid email format");
        });
    }
}