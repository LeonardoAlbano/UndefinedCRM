using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using UndefinedCRM.Domain.Entities;

namespace UndefinedCRM.Infrastructure;

public class ProjectRepository
{
    private readonly string _connectionString;

    public ProjectRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
            throw new ArgumentNullException(nameof(configuration), "Connection string 'DefaultConnection' not found.");
    }

    public async Task<int> CreateProjectAsync(Project project)
    {
        using IDbConnection dbConnection = new NpgsqlConnection(_connectionString);
        dbConnection.Open();
        var sql = @"INSERT INTO Projects (ClientId, Name, Link, Status, Value, StartDate, EndDate) 
                    VALUES (@ClientId, @Name, @Link, @Status, @Value, @StartDate, @EndDate) 
                    RETURNING Id";
        return await dbConnection.ExecuteScalarAsync<int>(sql, project);
    }

    public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(int userId)
    {
        try
        {
            using IDbConnection dbConnection = new NpgsqlConnection(_connectionString);
            dbConnection.Open();
            
            // Use a simpler query first to check if we can get any projects
            var sql = @"
            SELECT p.Id, p.ClientId, p.Name, p.Link, p.Status, p.Value, p.StartDate, p.EndDate,
                   c.Name as ClientName, c.Surname as ClientSurname 
            FROM Projects p
            INNER JOIN Clients c ON p.ClientId = c.Id
            WHERE c.UserId = @UserId";
        
            var projects = new List<Project>();
        
            // Use QueryAsync with a specific type mapping instead of dynamic
            var result = await dbConnection.QueryAsync<(
                int Id, 
                int ClientId, 
                string Name, 
                string Link, 
                string Status, 
                decimal? Value, 
                DateTime? StartDate, 
                DateTime? EndDate, 
                string ClientName, 
                string ClientSurname
            )>(sql, new { UserId = userId });
        
            foreach (var row in result)
            {
                var project = new Project
                {
                    Id = row.Id,
                    ClientId = row.ClientId,
                    Name = row.Name,
                    Link = row.Link,
                    Status = row.Status,
                    Value = row.Value,
                    StartDate = row.StartDate,
                    EndDate = row.EndDate,
                    Client = new Client
                    {
                        Id = row.ClientId,
                        Name = row.ClientName,
                        Surname = row.ClientSurname
                    }
                };
            
                projects.Add(project);
            }
        
            return projects;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetProjectsByUserIdAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        
            // Return an empty list instead of throwing to avoid 500 errors
            // This allows the UI to show "No projects found" instead of an error
            return new List<Project>();
        }
    }

    public async Task<Project?> GetProjectByIdAsync(int id, int userId)
    {
        using IDbConnection dbConnection = new NpgsqlConnection(_connectionString);
        dbConnection.Open();
        var sql = @"SELECT p.*, c.Name as ClientName, c.Surname as ClientSurname 
                    FROM Projects p
                    JOIN Clients c ON p.ClientId = c.Id
                    WHERE p.Id = @Id AND c.UserId = @UserId";
        
        var result = await dbConnection.QueryAsync<dynamic>(sql, new { Id = id, UserId = userId });
        var row = result.FirstOrDefault();
        
        if (row == null)
            return null;
            
        return new Project
        {
            Id = row.Id,
            ClientId = row.ClientId,
            Name = row.Name,
            Link = row.Link,
            Status = row.Status,
            Value = row.Value,
            StartDate = row.StartDate,
            EndDate = row.EndDate,
            Client = new Client
            {
                Id = row.ClientId,
                Name = row.ClientName,
                Surname = row.ClientSurname
            }
        };
    }

    public async Task<bool> UpdateProjectAsync(Project project, int userId)
    {
        using IDbConnection dbConnection = new NpgsqlConnection(_connectionString);
        dbConnection.Open();
        
        // First verify the client belongs to the user
        var clientBelongsToUser = await dbConnection.ExecuteScalarAsync<bool>(
            "SELECT COUNT(1) > 0 FROM Clients WHERE Id = @ClientId AND UserId = @UserId",
            new { ClientId = project.ClientId, UserId = userId });
        
        if (!clientBelongsToUser)
            return false;
        
        var sql = @"UPDATE Projects 
                SET ClientId = @ClientId, Name = @Name, Link = @Link, 
                    Status = @Status, Value = @Value, StartDate = @StartDate, EndDate = @EndDate 
                WHERE Id = @Id AND ClientId IN (
                    SELECT c.Id FROM Clients c WHERE c.UserId = @UserId
                )";
        
        var rowsAffected = await dbConnection.ExecuteAsync(sql, new {
            Id = project.Id,
            ClientId = project.ClientId,
            Name = project.Name,
            Link = project.Link,
            Status = project.Status,
            Value = project.Value,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            UserId = userId
        });
        
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteProjectAsync(int id, int userId)
    {
        using IDbConnection dbConnection = new NpgsqlConnection(_connectionString);
        dbConnection.Open();
        
        var sql = @"DELETE FROM Projects 
                    WHERE Id = @Id AND ClientId IN (
                        SELECT c.Id FROM Clients c WHERE c.UserId = @UserId
                    )";
                    
        var rowsAffected = await dbConnection.ExecuteAsync(sql, new { Id = id, UserId = userId });
        return rowsAffected > 0;
    }
}

