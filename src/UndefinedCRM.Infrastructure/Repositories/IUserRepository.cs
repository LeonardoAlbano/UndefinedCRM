using UndefinedCRM.Domain.Entities;

namespace UndefinedCRM.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<int> CreateUserAsync(User user);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByGoogleIdAsync(string googleId);
        Task<int> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
    }
}