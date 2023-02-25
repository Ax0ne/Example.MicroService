using Consul;
using Example.WebApi.Services;

namespace Example.WebApi.Extensions
{
    public static class ExampleExtensions
    {
        /// <summary>
        /// 注册Consul并且程序结束时移除Consul注册
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IConfiguration configuration)
        {
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            var consulClient = new ConsulClient(c =>
            {
                c.Address = new Uri(configuration["Consul:Address"]);
                //c.Datacenter = "dc1";
            });

            var host = configuration["Consul:CurrentHost"];
            var port = configuration["port"]; // 启动参数获取
            port = string.IsNullOrWhiteSpace(port) ? configuration["Consul:CurrentPort"] : port;
            var serviceId = $"service:{host}:{port}";
            consulClient.Agent.ServiceRegister(new AgentServiceRegistration
            {
                ID = serviceId,
                Name = configuration["Consul:ServiceName"],
                Address = host,
                Port = int.Parse(port),
                Tags = new[] { "api站点" },
                Check = new AgentServiceCheck
                {
                    Interval = TimeSpan.FromSeconds(5), // 心跳检查间隔时间
                    HTTP = $"{host}:{port}/api/example", // 心跳检查地址
                    Timeout = TimeSpan.FromSeconds(10), // 超时
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30), // 服务停止后多久注销服务
                }
            }).Wait();
            lifetime.ApplicationStopping.Register(() => { consulClient.Agent.ServiceDeregister(serviceId); });
            return app;
        }

        /// <summary>
        /// 添加Consulservice的依赖注入，以使用Consul的服务发现
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddConsulService(this IServiceCollection services)
        {
            services.AddTransient<IConsulService,ConsulService>();
            return services;
        }
    }
}