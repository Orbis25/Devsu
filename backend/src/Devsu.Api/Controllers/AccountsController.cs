using Devsu.Application.Dtos.Accounts;
using Devsu.Application.Dtos.Core;
using Devsu.Application.Services.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace Devsu.API.Controllers;
/// <summary>
/// 
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AccountsController : CoreController<IAccountService,GetAccount>
{

    private readonly IAccountService _service;
    /// <summary>
    /// </summary>
    /// <param name="service"></param>
    public AccountsController(IAccountService service) : base(service)
    {
        _service = service;
    }
    
    
    /// <summary>
    /// Create a new 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(Response<GetAccount>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateAccount input,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.CreateAsync(input, cancellationToken);

        if (!result.IsSuccess) return BadRequest(result);

        return CreatedAtAction(nameof(Get), new { Id = result.Data }, result);
    }
    

    /// <summary>
    /// update 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="input"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Response), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] EditAccount input,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.UpdateAsync(id,input, cancellationToken);

        if (!result.IsSuccess) return BadRequest(result);

        return NoContent();
    }

}