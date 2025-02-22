using Microsoft.AspNetCore.Mvc;
using UndefinedCRM.Application.UseCases.Users.Register;
using UndefinedCRM.Communication.Requests;

namespace UndefinedCRM.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost]
        public IActionResult Register([FromBody] RequestRegisterUserJson request)
        {
            var useCase = new RegisterUserUseCase();
            
            var response = useCase.Execute(request);
            
            return Created(string.Empty, response);
        }
    }
}
