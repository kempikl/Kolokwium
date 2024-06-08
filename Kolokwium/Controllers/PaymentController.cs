using Kolokwium.Dtos;
using Kolokwium.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public PaymentController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpPost]
    public async Task<ActionResult<int>> AddSubscriptionPayment(PaymentDto paymentDto)
    {
        try
        {
            var paymentId = await _subscriptionService.AddSubscriptionPayment(paymentDto);
            return CreatedAtAction(nameof(AddSubscriptionPayment), new { id = paymentId }, paymentId);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}