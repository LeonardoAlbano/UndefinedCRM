using FluentValidation.Results;
using UndefinedCRM.Communication.Requests;
using UndefinedCRM.Communication.Responses;
using UndefinedCRM.Exception;
using UndefinedCRM.Infrastructure;
using UndefinedCRM.Domain.Entities;
using UndefinedCRM.Infrastructure.Security.Cryptography;
using UndefinedCRM.Infrastructure.Security.Tokens.Access;

namespace UndefinedCRM.Application.UseCases.Users.Register;

public class RegisterUserUseCase
{
    private readonly UserRepository _userRepository;
    private readonly JwtTokenGenerator _tokenGenerator;

    public RegisterUserUseCase(UserRepository userRepository, JwtTokenGenerator tokenGenerator)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<ResponseRegisteredUserJson> Execute(RequestUserJson request)
    {
        await Validate(request);

        var cryptography = new BcryptAlgorithme();

        var entity = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = cryptography.HashPassword(request.Password),
        };

        var userId = await _userRepository.CreateUserAsync(entity);
        entity.Id = userId; 
        
        var token = _tokenGenerator.Generate(entity);

        return new ResponseRegisteredUserJson
        {
            Email = entity.Email,
            AccessToken = token
        };
    }

    private async Task Validate(RequestUserJson request)
    {
        var validator = new RegisterUserValidator();
        var result = validator.Validate(request);
        
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
        if (existingUser != null)
        {
            result.Errors.Add(new ValidationFailure("Email", "Email is already in use."));
        }

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
}

