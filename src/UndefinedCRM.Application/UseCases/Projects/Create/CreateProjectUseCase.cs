using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UndefinedCRM.Communication.Requests;
using UndefinedCRM.Communication.Responses;
using UndefinedCRM.Domain.Entities;
using UndefinedCRM.Exception;
using UndefinedCRM.Infrastructure;

namespace UndefinedCRM.Application.UseCases.Projects.Create;

public class CreateProjectUseCase
{
    private readonly ProjectRepository _projectRepository;
    private readonly ClientRepository _clientRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateProjectUseCase(
        ProjectRepository projectRepository, 
        ClientRepository clientRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _projectRepository = projectRepository;
        _clientRepository = clientRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ResponseProjectJson> Execute(RequestProjectJson request)
    {
        await Validate(request);
        
        var userId = GetAuthenticatedUserId();
        
        // Verify the client belongs to the user
        var client = await _clientRepository.GetClientByIdAsync(request.ClientId, userId);
        if (client == null)
        {
            throw new ErrorOnValidationException(new List<string> { "Client not found or does not belong to the user" });
        }
        
        var project = new Project
        {
            ClientId = request.ClientId,
            Name = request.Name,
            Link = request.Link,
            Status = request.Status,
            Value = request.Value,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };
        
        var projectId = await _projectRepository.CreateProjectAsync(project);
        project.Id = projectId;
        
        return new ResponseProjectJson
        {
            Id = project.Id,
            ClientId = project.ClientId,
            ClientName = $"{client.Name} {client.Surname}",
            Name = project.Name,
            Link = project.Link,
            Status = project.Status,
            Value = project.Value,
            StartDate = project.StartDate,
            EndDate = project.EndDate
        };
    }
    
    private async Task Validate(RequestProjectJson request)
    {
        var validator = new ProjectValidator();
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

