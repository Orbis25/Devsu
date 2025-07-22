using Microsoft.AspNetCore.Mvc;

namespace Devsu.API.Controllers;

/// <summary>
/// Controller for clients
/// </summary>
[ApiController]
[Route("api/clientes")]
public class ClientController : ControllerBase
{

    public ClientController()
    {
        
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok();
    }
    
}