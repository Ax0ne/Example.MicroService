using System.Text;
using Example.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using Polly.Fallback;
using System.Text.Encodings.Web;

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

        #region Polly
        [HttpGet("polly/retry")]
        public ActionResult Retry()
        {
            RetryPolicy policy = Policy.Handle<Exception>().WaitAndRetry(new[]
            {
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(8),
            },(exception,timeSpan)=>
            {
                _logger.LogWarning($"TimeSpan:{timeSpan},Exception:{exception.Message}");
            });
            policy.Execute(() =>
            {
                var i = 0;
                i++;
                i--;
                var r = 10 / i;
                _logger.LogWarning("Retry Complete");
            });
            return new JsonResult("");
        }
        [HttpGet("polly/breaker")]
        public ActionResult Breaker()
        {
            // 出错三次后熔断10秒
            //CircuitBreakerPolicy policy = Policy.Handle<Exception>().CircuitBreaker(3, TimeSpan.FromSeconds(10));
            // 3秒内5次请求失败率达到50%熔断10秒
            CircuitBreakerPolicy policy = Policy.Handle<Exception>().AdvancedCircuitBreaker(0.5, TimeSpan.FromSeconds(3), 5, TimeSpan.FromSeconds(10));

            for (var j = 0; j < 10; j++)
            {
                try
                {
                    policy.Execute(() =>
                    {
                        var i = 0;
                        i++;
                        i--;
                        var r = 10 / i;
                        _logger.LogWarning("CircuitBreaker Complete");
                    });
                }
                catch (Exception e)
                {
                    _logger.LogWarning($"第[{j}]次异常.{e.Message}");
                }
                //Thread.Sleep(1000);
            }
            return new JsonResult("");
        }

        [HttpGet("polly/fallback")]
        public ActionResult Fallback()
        {
            FallbackPolicy<string> policy = Policy<string>.Handle<Exception>().Fallback("默认值");
            var result = policy.Execute(() =>
            {
                throw new Exception("出错了");
                return "实际结果";
            });
            return new JsonResult(result,new System.Text.Json.JsonSerializerOptions{ Encoder= JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        }
        #endregion

        [HttpGet("test")]
        public async Task<JsonResult> Test()
        {
            await Task.Delay(3300);
            var domain = await _consulService.GetServiceAddress("WebApi");
            return new JsonResult(domain);
        }
        [HttpGet("test1")]
        public async Task<JsonResult> Test1()
        {
            var i = 0;
            i = 1 + i - 1;
            var r = DateTime.Now.Millisecond / i;
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