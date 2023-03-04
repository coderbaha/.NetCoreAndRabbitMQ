using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Entity.Business;
using Helper;
using Helper.Constant;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Publisher.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMQClientService _rabbitMQClientService;
        private IModel _channel;
        private IConfiguration _configuration;
        private string _pdfFilesPath;
        public Worker(ILogger<Worker> logger, RabbitMQClientService rabbitMQClientService, IConfiguration configuration)
        {
            _logger = logger;
            _rabbitMQClientService = rabbitMQClientService;
            _configuration = configuration;
            _pdfFilesPath = _configuration["PdfFilesPath"];
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
            _channel.BasicConsume(HelperConstants.PdfQueueName, false, consumer);
            consumer.Received += Consumer_Recived;

            return Task.CompletedTask;
        }
        private Task Consumer_Recived(object sender, BasicDeliverEventArgs @event)
        {
            Task.Delay(5000).Wait();

            try
            {
                var response = JsonSerializer.Deserialize<List<RabbitMQFileModel>>
                (Encoding.UTF8.GetString(@event.Body.ToArray()));
                foreach (var item in response)
                {
                    PdfCreater(_pdfFilesPath, item.Data, item.Filename);
                }

                _channel.BasicAck(@event.DeliveryTag, false);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message);
            }

            return Task.CompletedTask;
        }
        public void PdfCreater(string path, Byte[] data, Guid guid)
        {
            try
            {
                File.WriteAllBytes(path + guid + ".pdf", data);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, ExceptionHelper.PdfCreater + ":" + ex.Message);
            }
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
