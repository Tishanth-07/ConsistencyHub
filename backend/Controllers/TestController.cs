using Microsoft.AspNetCore.Mvc;
using ConsistencyHub.Models;
using ConsistencyHub.Services;

namespace ConsistencyHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController(TestService testService) : ControllerBase
    {
        private readonly TestService _testService = testService;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tests = await _testService.GetAsync();
            return Ok(tests);
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            var test = new Test { Message = "test number 1" };
            await _testService.CreateAsync(test);
            return Ok(test);
        }
    }
}
