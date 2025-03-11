using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UndefinedCRM.Communication.Requests;
using UndefinedCRM.Communication.Responses;
using UndefinedCRM.Domain.Entities;
using UndefinedCRM.Exception;
using UndefinedCRM.Infrastructure;

namespace UndefinedCRM.Application.UseCases.Clients.Create;

public class CreateClientUseCase
{
    private readonly ClientRepository _clientRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateClientUseCase(ClientRepository clientRepository, IHttpContextAccessor httpContextAccessor)
    {
        _clientRepository = clientRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ResponseClientJson> Execute(RequestClientJson request)
    {
        await Validate(request);
        
        var userId = GetAuthenticatedUserId();
        
        var client = new Client
        {
            UserId = userId,
            Name = request.Name,
            Surname = request.Surname,
            Email = request.Email,
            Phone = request.Phone
        };
        
        var clientId = await _clientRepository.CreateClientAsync(client);
        client.Id = clientId;
        
        return new ResponseClientJson
        {
            Id = client.Id,
            Name = client.Name,
            Surname = client.Surname,
            Email = client.Email,
            Phone = client.Phone
        };
    }
    
    private async Task Validate(RequestClientJson request)
    {
        var validator = new ClientValidator();
        var result = validator.Validate(request);
        
        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();
            throw new ErrorOnValidationException(errorMessages);
        }
    }
    
    private int GetAuthenticatedUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
        
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new ErrorOnValidationException(new List<string> { "User not authenticated" });
        }

        return userId;
    }
}

