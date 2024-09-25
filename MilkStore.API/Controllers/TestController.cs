using Microsoft.AspNetCore.Mvc;
using MilkStore.Core.Base;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly TestService _testService;
    public TestController(TestService testService)
    {
        _testService = testService;
    }
    [HttpGet]
    public async Task<IActionResult> Test1([FromQuery] int a)
    {
        int test = await _testService.Test(a);
        return Ok(BaseResponse<int>.OkResponse(test));
    }
}