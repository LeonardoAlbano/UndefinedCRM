namespace UndefinedCRM.Communication.Responses;

public class ResponseProjectJson
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string? Status { get; set; }
    public decimal? Value { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}