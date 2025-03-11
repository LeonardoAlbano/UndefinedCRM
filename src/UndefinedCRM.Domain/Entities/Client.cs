namespace UndefinedCRM.Domain.Entities;

public class Client
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    
    // Navigation property
    public User? User { get; set; }
}