using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UndefinedCRM.Exception;
using UndefinedCRM.Infrastructure;

namespace UndefinedCRM.Application.UseCases.Projects.Delete;

public class DeleteProjectUseCase
{
    private readonly ProjectRepository _projectRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteProjectUseCase(ProjectRepository projectRepository, IHttpContextAccessor httpContextAccessor)
    {
        _projectRepository = projectRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task Execute(int id)
    {
        var userId = GetAuthenticatedUserId();
        
        // Verify the project exists and belongs to a client of the user
        var existingProject = await _projectRepository.GetProjectByIdAsync(id, userId);
        if (existingProject == null)
        {
            throw new ErrorOnValidationException(new List<string> { "Project not found or does not belong to the user's clients" });
        }
        
        var success = await _projectRepository.DeleteProjectAsync(id, userId);
        if (!success)
        {
            throw new ErrorOnValidationException(new List<string> { "Failed to delete project" });
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