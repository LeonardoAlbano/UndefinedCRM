using System.Net;
using UndefinedCRM.Exception.ExceptionsBase;

namespace UndefinedCRM.Exception;

public class ErrorOnValidationException : UndefinedException
{
    private readonly List<string> _errors;

    public ErrorOnValidationException(List<string> errorMessages)
    {
        _errors = errorMessages;
    }
    
    public override List<string> GetErrorMessages() => _errors;
    
    public override HttpStatusCode GetStatusCode() => HttpStatusCode.BadRequest;
}