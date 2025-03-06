using FluentValidation;
using UndefinedCRM.Communication.Requests;

namespace UndefinedCRM.Application.UseCases.Users.Login;

public class LoginUserValidator : AbstractValidator<RequestLoginJson>
{
    public LoginUserValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
            
        RuleFor(request => request.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}