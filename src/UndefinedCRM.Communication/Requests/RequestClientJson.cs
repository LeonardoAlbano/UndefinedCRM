namespace UndefinedCRM.Communication.Requests;

public class RequestClientJson
{
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
}