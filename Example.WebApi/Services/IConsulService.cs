namespace Example.WebApi.Services
{
    public interface IConsulService
    {
        Task<string> GetServiceAddress(string serviceName);
    }
}
