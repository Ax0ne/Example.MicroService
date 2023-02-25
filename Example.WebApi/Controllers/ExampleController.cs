using Example.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Example.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExampleController : ControllerBase
    {
        readonly ILogger<ExampleController> _logger;
        readonly IConsulService _consulService;

        public ExampleController(ILogger<ExampleController> logger, IConsulService consulService)
        {
            this._logger = logger;
            _consulService = consulService;
        }

        [HttpGet("test")]
        public async Task<JsonResult> Test()
        {
            var domain = await _consulService.GetServiceAddress("WebApi");
            return new JsonResult(domain);
        }

        public JsonResult Index()
        {
            _logger.LogInformation($"当前地址：{Request.Host}");
            return new JsonResult(new { Name = "Example.WebApi", Port = Request.Host.Port, DateTime = DateTime.Now });
        }
    }
}