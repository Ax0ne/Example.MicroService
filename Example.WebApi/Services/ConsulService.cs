using Consul;

namespace Example.WebApi.Services
{
    public class ConsulService : IConsulService
    {
        readonly IConfiguration _configuration;
        readonly IHttpClientFactory _httpClientFactory;

        public ConsulService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetServiceAddress(string serviceName)
        {
            ConsulClient client =
                new ConsulClient(new ConsulClientConfiguration
                {
                    Address = new Uri(_configuration["Consul:Address"])
                }, _httpClientFactory.CreateClient("Consul"));
            var result = await client.Health.Service(serviceName, string.Empty, true);
            var count = result.Response?.Length ?? 0;
            if (count <= 0)
            {
                var msg = $"[{DateTime.Now}] 获取Consul服务地址失败，[{serviceName}]没有可用服务";
                if (result.Response == null)
                    msg = "无法连接Consul服务，请检查Consul服务是否健康";
                throw new Exception(msg);
            }

            var service = result.Response[new Random().Next(0, count)];

            return $"https://{service.Service.Address}:{service.Service.Port}";
        }
    }
    //public class ServiceInfo
    //{
    //    public string Address { get; set; }
    //}
}