using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using UndefinedCRM.Communication.Responses;
using UndefinedCRM.Exception;
using UndefinedCRM.Infrastructure;

namespace UndefinedCRM.Application.UseCases.Users.GetProfile;

public class GetUserProfileUseCase
{
    private readonly UserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetUserProfileUseCase(UserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ResponseUserProfileJson> Execute()
    {
        var userId = GetAuthenticatedUserId();
        
        var user = await _userRepository.GetUserByIdAsync(userId);
        
        if (user == null)
        {
            throw new ErrorOnValidationException(new List<string> { "User not found" });
        }

        return new ResponseUserProfileJson
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
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