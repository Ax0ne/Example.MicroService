using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Example.OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        public JsonResult Index()
        {
            return new JsonResult(new { Name="OrderApi"});
        }
        public JsonResult GetOrder(int id)
        {
            return new JsonResult(new { OrderId = 1, ProductName = "电饭煲" });
        }
    }
}
