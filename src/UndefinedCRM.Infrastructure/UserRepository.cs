using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using UndefinedCRM.Domain;

namespace UndefinedCRM.Infrastructure;

public class UserRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") 
                                                ?? throw new ArgumentNullException(nameof(configuration), "Connection string 'DefaultConnection' not found.");

    public async Task<int> CreateUserAsync(User user)
    {
        using IDbConnection dbConnection = new NpgsqlConnection(_connectionString);
        dbConnection.Open();
        var sql = "INSERT INTO Users (Name, Email, Password) VALUES (@Name, @Email, @Password) RETURNING Id";
        return await dbConnection.ExecuteScalarAsync<int>(sql, user);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        using IDbConnection dbConnection = new NpgsqlConnection(_connectionString);
        dbConnection.Open();
        return await dbConnection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Email = @Email", new { Email = email });
    }
}