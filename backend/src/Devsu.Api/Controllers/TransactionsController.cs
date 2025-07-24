using Devsu.Application.Dtos.Core;
using Devsu.Application.Dtos.Transactions;
using Devsu.Application.Services.Transactions;
using Microsoft.AspNetCore.Mvc;

namespace Devsu.API.Controllers;

/// <summary>
/// </summary>
[Route("api/[controller]")]
public class TransactionsController : CoreController<ITransactionService, GetTransaction>
{
    private readonly ITransactionService _service;

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="service"></param>
    public TransactionsController(ITransactionService service) : base(service)
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
    [ProducesResponseType(typeof(Response<GetTransaction>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTransaction input,
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
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] EditTransaction input,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.UpdateAsync(id, input, cancellationToken);

        if (!result.IsSuccess) return BadRequest(result);

        return NoContent();
    }

 
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("report/pdf")]
    [ProducesResponseType(typeof(Response), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult ReportPdf(CancellationToken cancellationToken = default)
    {
        var result = _service.ExportTransactionReport();
        if (!result.IsSuccess) return BadRequest(result);

        return Ok(result);
    }
}