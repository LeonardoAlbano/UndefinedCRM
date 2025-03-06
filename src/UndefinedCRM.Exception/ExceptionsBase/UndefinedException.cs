using System.Net;

namespace UndefinedCRM.Exception.ExceptionsBase;

public abstract class UndefinedException : System.Exception
{
    public abstract List<string> GetErrorMessages();
    public abstract HttpStatusCode GetStatusCode();
}