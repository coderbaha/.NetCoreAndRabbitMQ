using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Publisher.Services;
using RabbitMQ.Client;

namespace Consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration Configuration = hostContext.Configuration;
                    services.AddHostedService<Worker>();
                    services.AddSingleton<RabbitMQClientService>();
                    services.AddSingleton(sp => new ConnectionFactory()
                    {
                        HostName = Configuration.GetConnectionString("RabbitMQ"),
                        VirtualHost = "/"
                        ,
                        UserName = "guest",
                        Password = "guest"
                        ,
                        DispatchConsumersAsync = true
                    });
                });
    }
}
