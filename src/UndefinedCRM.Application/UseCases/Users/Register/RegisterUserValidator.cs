using FluentValidation;
using UndefinedCRM.Communication.Requests;

namespace UndefinedCRM.Application.UseCases.Users.Register;

public class RegisterUserValidator : AbstractValidator<RequestUserJson>
{
    public RegisterUserValidator()
    {
        RuleFor(request => request.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(request => request.Email).NotEmpty().WithMessage("Email is required");
        RuleFor(request => request.Password).NotEmpty().WithMessage("Password is required");
        When(request => string.IsNullOrEmpty(request.Password) == false, () =>
        {
            RuleFor(request => request.Password.Length).GreaterThanOrEqualTo(6)
                .WithMessage("Password must contain more than 6 characters");
        });
    }
}