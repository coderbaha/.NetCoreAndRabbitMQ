using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Publisher.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace UserRequestConsumer
{
    public class Worker : BackgroundService
    {
        private readonly RabbitMQClientService _rabbitMQClientService;
        private IModel _channel;
        private IConfiguration _configuration;
        public Worker(RabbitMQClientService rabbitMQClientService, IConfiguration configuration)
        {
            _rabbitMQClientService = rabbitMQClientService;
            _configuration = configuration;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMQClientService.Connect();
            _channel.BasicQos(0, 1, false);

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume("UsersCreaterQueue", false, consumer);
            consumer.Received += Consumer_Recived;

            return Task.CompletedTask;
        }
        private Task Consumer_Recived(object sender, BasicDeliverEventArgs @event)
        {
            Task.Delay(5000).Wait();

            try
            {
                var response = JsonSerializer.Deserialize<string>(Encoding.UTF8.GetString(@event.Body.ToArray()));
                foreach (var item in response)
                {
                    ServiceRequest("https://localhost:44301/api/DailyDocument/PdfCreateTriggerByCreateUser?createUser="+ response);
                }

                _channel.BasicAck(@event.DeliveryTag, false);
            }
            catch (Exception ex)
            {
            }

            return Task.CompletedTask;
        }
        public void ServiceRequest(string url)
        {
            using (var client = new HttpClient())
            {
                var result = client.GetAsync(url).Result;
            }
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
