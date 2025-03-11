using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UndefinedCRM.Communication.Responses;
using UndefinedCRM.Exception;
using UndefinedCRM.Infrastructure;

namespace UndefinedCRM.Application.UseCases.Projects.GetAll;

public class GetAllProjectsUseCase
{
    private readonly ProjectRepository _projectRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetAllProjectsUseCase(ProjectRepository projectRepository, IHttpContextAccessor httpContextAccessor)
    {
        _projectRepository = projectRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<ResponseProjectJson>> Execute()
    {
        try
        {
            var userId = GetAuthenticatedUserId();
        
            var projects = await _projectRepository.GetProjectsByUserIdAsync(userId);
        
            return projects.Select(project => new ResponseProjectJson
            {
                Id = project.Id,
                ClientId = project.ClientId,
                ClientName = project.Client != null ? $"{project.Client.Name} {project.Client.Surname}" : "",
                Name = project.Name,
                Link = project.Link,
                Status = project.Status,
                Value = project.Value,
                StartDate = project.StartDate,
                EndDate = project.EndDate
            }).ToList();
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error in GetAllProjectsUseCase.Execute: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        
            // Return an empty list instead of throwing
            return new List<ResponseProjectJson>();
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
