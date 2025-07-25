using Devsu.Application.Dtos.Core;
using Devsu.Application.Dtos.Users;
using Devsu.Application.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace Devsu.API.Controllers;

/// <summary>
/// Controller for clients
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : CoreController<IUserService,GetUser>
{
    private readonly IUserService _userService;

    /// <summary>
    /// </summary>
    /// <param name="userService"></param>
    public UsersController(IUserService userService):base(userService)
    {
        _userService = userService;
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
    /// update a user
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Response), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] EditUser input,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.UpdateAsync(id,input, cancellationToken);

        if (!result.IsSuccess) return BadRequest(result);

        return NoContent();
    }

    
}