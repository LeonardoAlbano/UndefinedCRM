using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UndefinedCRM.Application.UseCases.Clients.Create;
using UndefinedCRM.Application.UseCases.Clients.Delete;
using UndefinedCRM.Application.UseCases.Clients.GetAll;
using UndefinedCRM.Application.UseCases.Clients.Update;
using UndefinedCRM.Communication.Requests;
using UndefinedCRM.Communication.Responses;
using UndefinedCRM.Exception.ExceptionsBase;

namespace UndefinedCRM.Api.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly CreateClientUseCase _createClientUseCase;
    private readonly GetAllClientsUseCase _getAllClientsUseCase;
    private readonly UpdateClientUseCase _updateClientUseCase;
    private readonly DeleteClientUseCase _deleteClientUseCase;

    public ClientsController(
        CreateClientUseCase createClientUseCase,
        GetAllClientsUseCase getAllClientsUseCase,
        UpdateClientUseCase updateClientUseCase,
        DeleteClientUseCase deleteClientUseCase)
    {
        _createClientUseCase = createClientUseCase;
        _getAllClientsUseCase = getAllClientsUseCase;
        _updateClientUseCase = updateClientUseCase;
        _deleteClientUseCase = deleteClientUseCase;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseClientJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(RequestClientJson request)
    {
        try
        {
            var response = await _createClientUseCase.Execute(request);
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

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ResponseClientJson>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var response = await _getAllClientsUseCase.Execute();
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

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ResponseClientJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, RequestClientJson request)
    {
        try
        {
            var response = await _updateClientUseCase.Execute(id, request);
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

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResponseErrorMessageJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _deleteClientUseCase.Execute(id);
            return NoContent();
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

