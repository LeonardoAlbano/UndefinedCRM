using UndefinedCRM.Communication.Requests;
using UndefinedCRM.Communication.Responses;
using UndefinedCRM.Exception;
using UndefinedCRM.Infrastructure;
using UndefinedCRM.Domain;

namespace UndefinedCRM.Application.UseCases.Users.Register;

public class RegisterUserUseCase
{
    private readonly UserRepository _userRepository;

    public RegisterUserUseCase(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ResponseRegisteredUserJson> Execute(RequestUserJson request)
    {
        Validate(request);

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = request.Password // Sem hash por enquanto
        };

        var userId = await _userRepository.CreateUserAsync(user);

        return new ResponseRegisteredUserJson
        {
            Email = user.Email,
            AccessToken = "fake-token" // Token fake para teste
        };
    }

    private void Validate(RequestUserJson request)
    {
        var validator = new RegisterUserValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}