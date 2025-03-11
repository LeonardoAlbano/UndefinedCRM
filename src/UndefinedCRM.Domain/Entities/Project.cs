namespace UndefinedCRM.Domain.Entities;

public class Project
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string? Status { get; set; }
    public decimal? Value { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    // Navigation property
    public Client? Client { get; set; }
}