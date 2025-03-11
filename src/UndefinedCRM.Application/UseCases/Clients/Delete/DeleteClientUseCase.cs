using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UndefinedCRM.Exception;
using UndefinedCRM.Infrastructure;

namespace UndefinedCRM.Application.UseCases.Clients.Delete;

public class DeleteClientUseCase
{
    private readonly ClientRepository _clientRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteClientUseCase(ClientRepository clientRepository, IHttpContextAccessor httpContextAccessor)
    {
        _clientRepository = clientRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task Execute(int id)
    {
        var userId = GetAuthenticatedUserId();
        
        var existingClient = await _clientRepository.GetClientByIdAsync(id, userId);
        if (existingClient == null)
        {
            throw new ErrorOnValidationException(new List<string> { "Client not found" });
        }
        
        var success = await _clientRepository.DeleteClientAsync(id, userId);
        if (!success)
        {
            throw new ErrorOnValidationException(new List<string> { "Failed to delete client" });
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