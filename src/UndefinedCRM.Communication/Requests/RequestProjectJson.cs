namespace UndefinedCRM.Communication.Requests;

public class RequestProjectJson
{
    public int ClientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string? Status { get; set; }
    public decimal? Value { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}