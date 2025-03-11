using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UndefinedCRM.Communication.Responses;
using UndefinedCRM.Exception;
using UndefinedCRM.Infrastructure;

namespace UndefinedCRM.Application.UseCases.Clients.GetAll;

public class GetAllClientsUseCase
{
    private readonly ClientRepository _clientRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetAllClientsUseCase(ClientRepository clientRepository, IHttpContextAccessor httpContextAccessor)
    {
        _clientRepository = clientRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IEnumerable<ResponseClientJson>> Execute()
    {
        var userId = GetAuthenticatedUserId();
        
        var clients = await _clientRepository.GetClientsByUserIdAsync(userId);
        
        return clients.Select(client => new ResponseClientJson
        {
            Id = client.Id,
            Name = client.Name,
            Surname = client.Surname,
            Email = client.Email,
            Phone = client.Phone
        });
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