using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UndefinedCRM.Application.UseCases.Users.GetProfile;
using UndefinedCRM.Application.UseCases.Users.GoogleAuth;
using UndefinedCRM.Application.UseCases.Users.Login;
using UndefinedCRM.Application.UseCases.Users.Register;
using UndefinedCRM.Communication.Requests;
using UndefinedCRM.Communication.Responses;
using UndefinedCRM.Exception.ExceptionsBase;

namespace UndefinedCRM.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly RegisterUserUseCase _registerUserUseCase;
        private readonly LoginUserUseCase _loginUserUseCase;
        private readonly GetUserProfileUseCase _getUserProfileUseCase;
        private readonly GoogleAuthUseCase _googleAuthUseCase;

        public UsersController(
            RegisterUserUseCase registerUserUseCase,
            LoginUserUseCase loginUserUseCase,
            GetUserProfileUseCase getUserProfileUseCase,
            GoogleAuthUseCase googleAuthUseCase)
        {
            _registerUserUseCase = registerUserUseCase;
            _loginUserUseCase = loginUserUseCase;
            _getUserProfileUseCase = getUserProfileUseCase;
            _googleAuthUseCase = googleAuthUseCase;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseRegisteredUserJson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RequestUserJson request)
        {
            try
            {
                var response = await _registerUserUseCase.Execute(request);
                return Created(string.Empty, response);
            }
            catch (UndefinedException ex)
            {
                return BadRequest(new ResponseErrorMessageJson { Errors = ex.GetErrorMessages() });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorMessageJson { Errors = ["Internal Server Error: " + ex.Message] });
            }
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ResponseLoginJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(RequestLoginJson request)
        {
            try
            {
                var response = await _loginUserUseCase.Execute(request);
                return Ok(response);
            }
            catch (UndefinedException ex)
            {
                return BadRequest(new ResponseErrorMessageJson { Errors = ex.GetErrorMessages() });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorMessageJson { Errors = ["Internal Server Error: " + ex.Message] });
            }
        }
        
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResponseUserProfileJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var response = await _getUserProfileUseCase.Execute();
                return Ok(new { user = response });
            }
            catch (UndefinedException ex)
            {
                return BadRequest(new ResponseErrorMessageJson { Errors = ex.GetErrorMessages() });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorMessageJson { Errors = ["Internal Server Error: " + ex.Message] });
            }
        }
        
        [HttpPost("oauth/google")]
        [ProducesResponseType(typeof(ResponseGoogleAuthJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GoogleAuth(RequestGoogleAuthJson request)
        {
            try
            {
                var response = await _googleAuthUseCase.Execute(request);
                return Ok(response);
            }
            catch (UndefinedException ex)
            {
                return BadRequest(new ResponseErrorMessageJson { Errors = ex.GetErrorMessages() });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorMessageJson { Errors = ["Internal Server Error: " + ex.Message] });
            }
        }
    }
}

