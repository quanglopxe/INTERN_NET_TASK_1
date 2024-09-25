using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> ProcessPayment(string id, int quatity)
    {

        return Ok();
    }
}