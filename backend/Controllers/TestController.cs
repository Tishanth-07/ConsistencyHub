using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Services;

namespace backend.Controllers
{
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
