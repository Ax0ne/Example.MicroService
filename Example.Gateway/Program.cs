using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

namespace Example.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new WebHostBuilder().UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true,
                            true)
                        .AddJsonFile("ocelot.json",false,true)
                        .AddEnvironmentVariables();
                }).ConfigureServices(cs =>
                {
                    cs.AddOcelot().AddConsul();
                }).ConfigureLogging(logging => { logging.AddConsole(); })
                //.UseIISIntegration()
                .Configure(app => { app.UseOcelot().Wait(); })
                .Build()
                .Run();
        }
    }
}