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
            
            var sql = @"
            SELECT p.Id, p.ClientId, p.Name, p.Link, p.Status, p.Value, p.StartDate, p.EndDate,
                   c.Name as ClientName, c.Surname as ClientSurname 
            FROM Projects p
            INNER JOIN Clients c ON p.ClientId = c.Id
            WHERE c.UserId = @UserId";
        
            var projects = new List<Project>();
        
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
        
            return new List<Project>();
        }
    }

    public async Task<Project?> GetProjectByIdAsync(int id, int userId)
    {
        try
        {
            using IDbConnection dbConnection = new NpgsqlConnection(_connectionString);
            dbConnection.Open();
            var sql = @"
            SELECT p.Id, p.ClientId, p.Name, p.Link, p.Status, p.Value, p.StartDate, p.EndDate,
                   c.Name as ClientName, c.Surname as ClientSurname 
            FROM Projects p
            INNER JOIN Clients c ON p.ClientId = c.Id
            WHERE p.Id = @Id AND c.UserId = @UserId";
            
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
            )>(sql, new { Id = id, UserId = userId });
            
            var row = result.FirstOrDefault();
            
            if (row.Id == 0) // Default value for int when no row is found
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetProjectByIdAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return null;
        }
    }

    public async Task<bool> UpdateProjectAsync(Project project, int userId)
    {
        try
        {
            using IDbConnection dbConnection = new NpgsqlConnection(_connectionString);
            dbConnection.Open();
            
            var clientBelongsToUser = await dbConnection.ExecuteScalarAsync<bool>(
                "SELECT COUNT(1) > 0 FROM Clients WHERE Id = @ClientId AND UserId = @UserId",
                new { ClientId = project.ClientId, UserId = userId });
            
            if (!clientBelongsToUser)
                return false;
            
            var sql = @"
            UPDATE Projects 
            SET ClientId = @ClientId, 
                Name = @Name, 
                Link = @Link, 
                Status = @Status, 
                Value = @Value, 
                StartDate = @StartDate, 
                EndDate = @EndDate 
            WHERE Id = @Id";
            
            var rowsAffected = await dbConnection.ExecuteAsync(sql, new {
                Id = project.Id,
                ClientId = project.ClientId,
                Name = project.Name,
                Link = project.Link,
                Status = project.Status,
                Value = project.Value,
                StartDate = project.StartDate,
                EndDate = project.EndDate
            });
            
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateProjectAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }

    public async Task<bool> DeleteProjectAsync(int id, int userId)
    {
        try
        {
            using IDbConnection dbConnection = new NpgsqlConnection(_connectionString);
            dbConnection.Open();
            
            var projectExists = await dbConnection.ExecuteScalarAsync<bool>(@"
                SELECT COUNT(1) > 0 
                FROM Projects p
                JOIN Clients c ON p.ClientId = c.Id
                WHERE p.Id = @Id AND c.UserId = @UserId", 
                new { Id = id, UserId = userId });
                
            if (!projectExists)
                return false;
            
            var sql = "DELETE FROM Projects WHERE Id = @Id";
                        
            var rowsAffected = await dbConnection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteProjectAsync: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }
}
