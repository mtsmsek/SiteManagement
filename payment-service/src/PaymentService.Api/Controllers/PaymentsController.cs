using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Payments;

namespace PaymentService.Api.Controllers;

/// <summary>
/// The gateway's charge endpoint. Thin: hands the request to the application
/// <see cref="IPaymentProcessor"/> and returns the settled result. Declines
/// (insufficient funds, rejected card) come back as a 200 with a Failed status
/// — they are valid business outcomes, not transport errors — so the caller
/// inspects <c>Status</c> rather than the HTTP code for the decision.
/// </summary>
[ApiController]
[Route("api/payments")]
public sealed class PaymentsController(IPaymentProcessor processor) : ControllerBase
{
    private readonly IPaymentProcessor _processor = processor;

    /// <summary>Processes a card charge (idempotent by the request's key).</summary>
    [HttpPost]
    [ProducesResponseType<ProcessPaymentApiResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProcessPaymentApiResponse>> Charge(
        [FromBody] ProcessPaymentApiRequest body,
        CancellationToken ct)
    {
        var result = await _processor.ProcessAsync(body.ToApplicationRequest(), ct);
        return Ok(ProcessPaymentApiResponse.From(result));
    }
}
