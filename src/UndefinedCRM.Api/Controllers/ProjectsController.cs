using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UndefinedCRM.Application.UseCases.Projects.Create;
using UndefinedCRM.Application.UseCases.Projects.Delete;
using UndefinedCRM.Application.UseCases.Projects.GetAll;
using UndefinedCRM.Application.UseCases.Projects.Update;
using UndefinedCRM.Communication.Requests;
using UndefinedCRM.Communication.Responses;
using UndefinedCRM.Exception.ExceptionsBase;

namespace UndefinedCRM.Api.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly CreateProjectUseCase _createProjectUseCase;
    private readonly GetAllProjectsUseCase _getAllProjectsUseCase;
    private readonly UpdateProjectUseCase _updateProjectUseCase;
    private readonly DeleteProjectUseCase _deleteProjectUseCase;

    public ProjectsController(
        CreateProjectUseCase createProjectUseCase,
        GetAllProjectsUseCase getAllProjectsUseCase,
        UpdateProjectUseCase updateProjectUseCase,
        DeleteProjectUseCase deleteProjectUseCase)
    {
        _createProjectUseCase = createProjectUseCase;
        _getAllProjectsUseCase = getAllProjectsUseCase;
        _updateProjectUseCase = updateProjectUseCase;
        _deleteProjectUseCase = deleteProjectUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseProjectJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(RequestProjectJson request)
    {
        try
        {
            var response = await _createProjectUseCase.Execute(request);
            return Created(string.Empty, response);
        }
        catch (UndefinedException ex)
        {
            return BadRequest(new ResponseErrorMessageJson { Errors = ex.GetErrorMessages() });
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error in Create: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorMessageJson { Errors = ["Internal Server Error: " + ex.Message] });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ResponseProjectJson>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var response = await _getAllProjectsUseCase.Execute();
            return Ok(response);
        }
        catch (UndefinedException ex)
        {
            return BadRequest(new ResponseErrorMessageJson { Errors = ex.GetErrorMessages() });
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error in GetAll: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        
            // Return an empty array instead of 500 error
            return Ok(new List<ResponseProjectJson>());
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ResponseProjectJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, RequestProjectJson request)
    {
        try
        {
            var response = await _updateProjectUseCase.Execute(id, request);
            return Ok(response);
        }
        catch (UndefinedException ex)
        {
            return BadRequest(new ResponseErrorMessageJson { Errors = ex.GetErrorMessages() });
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error in Update: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorMessageJson { Errors = ["Internal Server Error: " + ex.Message] });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _deleteProjectUseCase.Execute(id);
            return NoContent();
        }
        catch (UndefinedException ex)
        {
            return BadRequest(new ResponseErrorMessageJson { Errors = ex.GetErrorMessages() });
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"Error in Delete: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorMessageJson { Errors = ["Internal Server Error: " + ex.Message] });
        }
    }
}

