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
        try
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
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error in DeleteClientUseCase.Execute: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw; // Re-throw to let the controller handle it
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