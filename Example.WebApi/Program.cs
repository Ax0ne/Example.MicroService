using Example.WebApi.Extensions;

namespace Example.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // args 参数会被添加到Configuration里
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddLogging(c => c.AddConsole());
            builder.Services.AddHttpClient("Consul",c=>
            {
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            builder.Services.AddConsulService();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.UseConsul(app.Configuration);
            app.Run();
        }
    }
}