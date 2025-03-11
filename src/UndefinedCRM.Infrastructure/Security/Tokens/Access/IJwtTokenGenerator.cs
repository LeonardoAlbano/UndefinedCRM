using UndefinedCRM.Domain.Entities;

namespace UndefinedCRM.Infrastructure.Security.Tokens.Access
{
    public interface IJwtTokenGenerator
    {
        string Generate(User user);
    }
}