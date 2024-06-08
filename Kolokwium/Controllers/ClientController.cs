using Kolokwium.Dtos;
using Kolokwium.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public ClientController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet("{clientId}")]
    public async Task<ActionResult<ClientDto>> GetClientWithSubscriptions(int clientId)
    {
        try
        {
            var client = await _subscriptionService.GetClientWithSubscriptions(clientId);
            return Ok(client);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}