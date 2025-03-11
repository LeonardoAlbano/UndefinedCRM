namespace UndefinedCRM.Communication.Responses;

public class ResponseClientJson
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
}