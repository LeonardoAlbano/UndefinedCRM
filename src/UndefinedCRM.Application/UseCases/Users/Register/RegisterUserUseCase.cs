using UndefinedCRM.Communication.Requests;
using UndefinedCRM.Communication.Responses;

namespace UndefinedCRM.Application.UseCases.Users.Register;

public class RegisterUserUseCase
{
    public ResponseRegisteredUserJson Execute(RequestRegisterUserJson request)
    {
        Validate(request);
        
        return new ResponseRegisteredUserJson();
    }

    private void Validate(RequestRegisterUserJson request)
    {
        var nameIsEmpty = string.IsNullOrWhiteSpace(request.Name);

        if (nameIsEmpty)
        {
            throw new ArgumentException("O Nome é obrigatorio");
        }
    }
}