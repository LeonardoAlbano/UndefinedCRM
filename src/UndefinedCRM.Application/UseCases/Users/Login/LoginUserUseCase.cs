using FluentValidation.Results;
using UndefinedCRM.Communication.Requests;
using UndefinedCRM.Communication.Responses;
using UndefinedCRM.Exception;
using UndefinedCRM.Infrastructure;
using UndefinedCRM.Infrastructure.Security.Cryptography;
using UndefinedCRM.Infrastructure.Security.Tokens.Access;

namespace UndefinedCRM.Application.UseCases.Users.Login;

public class LoginUserUseCase
{
    private readonly UserRepository _userRepository;
    private readonly JwtTokenGenerator _tokenGenerator;

    public LoginUserUseCase(UserRepository userRepository, JwtTokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<ResponseLoginJson> Execute(RequestLoginJson request)
    {
        await Validate(request);

        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        
        if (user == null)
        {
            throw new ErrorOnValidationException(new List<string> { "Invalid email or password" });
        }

        var bcrypt = new BcryptAlgorithme();
        var passwordIsValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

        if (!passwordIsValid)
        {
            throw new ErrorOnValidationException(new List<string> { "Invalid email or password" });
        }

        // Generate JWT token
        var token = _tokenGenerator.Generate(user);

        return new ResponseLoginJson
        {
            Email = user.Email,
            Name = user.Name,
            AccessToken = token
        };
    }

    private async Task Validate(RequestLoginJson request)
    {
        var validator = new LoginUserValidator();
        var result = validator.Validate(request);

        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        
        if (user == null)
        {
            result.Errors.Add(new ValidationFailure("", "Invalid email or password"));
        }

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}

