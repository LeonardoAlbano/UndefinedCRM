using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using UndefinedCRM.Domain.Entities;

namespace UndefinedCRM.Infrastructure;

public class ClientRepository
{
    private readonly string _connectionString;

    public ClientRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
            throw new ArgumentNullException(nameof(configuration), "Connection string 'DefaultConnection' not found.");
    }

    public async Task<int> CreateClientAsync(Client client)
    {
        using IDbConnection dbConnection = new NpgsqlConnection(_connectionString);
        dbConnection.Open();
        var sql = @"INSERT INTO Clients (UserId, Name, Surname, Email, Phone) 
                    VALUES (@UserId, @Name, @Surname, @Email, @Phone) 
                    RETURNING Id";
        return await dbConnection.ExecuteScalarAsync<int>(sql, client);
    }

    public async Task<IEnumerable<Client>> GetClientsByUserIdAsync(int userId)
    {
        using IDbConnection dbConnection = new NpgsqlConnection(_connectionString);
        dbConnection.Open();
        return await dbConnection.QueryAsync<Client>(
            "SELECT * FROM Clients WHERE UserId = @UserId ORDER BY Name", 
            new { UserId = userId });
    }

    public async Task<Client?> GetClientByIdAsync(int id, int userId)
    {
        using IDbConnection dbConnection = new NpgsqlConnection(_connectionString);
        dbConnection.Open();
        return await dbConnection.QueryFirstOrDefaultAsync<Client>(
            "SELECT * FROM Clients WHERE Id = @Id AND UserId = @UserId", 
            new { Id = id, UserId = userId });
    }

    public async Task<bool> UpdateClientAsync(Client client)
    {
        using IDbConnection dbConnection = new NpgsqlConnection(_connectionString);
        dbConnection.Open();
        var sql = @"UPDATE Clients 
                    SET Name = @Name, Surname = @Surname, Email = @Email, Phone = @Phone 
                    WHERE Id = @Id AND UserId = @UserId";
        var rowsAffected = await dbConnection.ExecuteAsync(sql, client);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteClientAsync(int id, int userId)
    {
        try
        {
            using IDbConnection dbConnection = new NpgsqlConnection(_connectionString);
            dbConnection.Open();
            
            using var transaction = dbConnection.BeginTransaction();
            
            try
            {
                var hasProjects = await dbConnection.ExecuteScalarAsync<bool>(
                    "SELECT COUNT(1) > 0 FROM Projects WHERE ClientId = @Id", 
                    new { Id = id },
                    transaction);
                
                if (hasProjects)
                {
                    await dbConnection.ExecuteAsync(
                        "DELETE FROM Projects WHERE ClientId = @Id", 
                        new { Id = id },
                        transaction);
                }
                
                var rowsAffected = await dbConnection.ExecuteAsync(
                    "DELETE FROM Clients WHERE Id = @Id AND UserId = @UserId", 
                    new { Id = id, UserId = userId },
                    transaction);
                
                transaction.Commit();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error in DeleteClientAsync transaction: {ex.Message}");
                throw;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteClientAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }
}
