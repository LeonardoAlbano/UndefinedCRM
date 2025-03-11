using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UndefinedCRM.Communication.Requests;
using UndefinedCRM.Communication.Responses;
using UndefinedCRM.Domain.Entities;
using UndefinedCRM.Exception;
using UndefinedCRM.Infrastructure;

namespace UndefinedCRM.Application.UseCases.Projects.Update;

public class UpdateProjectUseCase
{
    private readonly ProjectRepository _projectRepository;
    private readonly ClientRepository _clientRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateProjectUseCase(
        ProjectRepository projectRepository, 
        ClientRepository clientRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _projectRepository = projectRepository;
        _clientRepository = clientRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ResponseProjectJson> Execute(int id, RequestProjectJson request)
    {
        try
        {
            await Validate(request);
            
            var userId = GetAuthenticatedUserId();
            
            // Verify the project exists and belongs to a client of the user
            var existingProject = await _projectRepository.GetProjectByIdAsync(id, userId);
            if (existingProject == null)
            {
                throw new ErrorOnValidationException(new List<string> { "Project not found or does not belong to the user's clients" });
            }
            
            // Verify the client belongs to the user
            var client = await _clientRepository.GetClientByIdAsync(request.ClientId, userId);
            if (client == null)
            {
                throw new ErrorOnValidationException(new List<string> { "Client not found or does not belong to the user" });
            }
            
            var project = new Project
            {
                Id = id,
                ClientId = request.ClientId,
                Name = request.Name,
                Link = request.Link,
                Status = request.Status,
                Value = request.Value,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };
            
            var success = await _projectRepository.UpdateProjectAsync(project, userId);
            if (!success)
            {
                throw new ErrorOnValidationException(new List<string> { "Failed to update project" });
            }
            
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
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error in UpdateProjectUseCase.Execute: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw; // Re-throw to let the controller handle it
        }
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

