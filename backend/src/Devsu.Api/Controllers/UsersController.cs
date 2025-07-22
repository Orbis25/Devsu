using Devsu.Application.Dtos.Core;
using Devsu.Application.Dtos.Users;
using Devsu.Application.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace Devsu.API.Controllers;

/// <summary>
/// Controller for clients
/// </summary>
[ApiController]
[Route("api/clientes")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// </summary>
    /// <param name="userService"></param>
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get al paginated users
    /// </summary>
    /// <param name="search"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(Response<PaginationResult<GetUser>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get([FromQuery] Paginate search, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetAllAsync(search, cancellationToken);
        return Ok(result);
    }


    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(Response<GetUser>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUser input,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.CreateAsync(input, cancellationToken);

        if (!result.IsSuccess) return BadRequest(result);

        return CreatedAtAction(nameof(Get), new { Id = result.Data }, result);
    }

    /// <summary>
    /// Get a user by Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}", Name = "Get")]
    [ProducesResponseType(typeof(Response<GetUser>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetByIdAsync(id, cancellationToken);
        if (result.IsNotFound) return NotFound(result);
        if (!result.IsSuccess) return BadRequest(result);
        return Ok(result);
    }
    
    /// <summary>
    /// soft delete user by Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _userService.RemoveAsync(id, cancellationToken);
        if (result.IsNotFound) return NotFound(result);
        if (!result.IsSuccess) return BadRequest(result);
        return NoContent();
    }

    /// <summary>
    /// update a user
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Response), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromRoute] Guid id, [FromBody] EditUser input,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.UpdateAsync(id,input, cancellationToken);

        if (!result.IsSuccess) return BadRequest(result);

        return NoContent();
    }

    
}