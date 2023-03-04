using System.Text;
using Entity.Business;
using Entity.Database;
using Helper.Constant;
using Newtonsoft.Json;
using Publisher.Services;
using RabbitMQ.Client;

namespace Publisher.Publisher
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _rabbitMQClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitMQClientService)
        {
            _rabbitMQClientService = rabbitMQClientService;
        }

        public void Publish<T>(T message)
        {
            var channel = _rabbitMQClientService.Connect();

            var bodyString = JsonConvert.SerializeObject(message);
            var bodyByte = Encoding.UTF8.GetBytes(bodyString);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.ExchangeDeclare(HelperConstants.ExchangePdfName, ExchangeType.Direct, true, false);
            channel.QueueDeclare(queue: HelperConstants.PdfQueueName, true, false, false);
            channel.QueueBind(exchange: HelperConstants.ExchangePdfName, queue: HelperConstants.PdfQueueName, routingKey: HelperConstants.RoutingPdf);
            channel.BasicPublish(exchange: HelperConstants.ExchangePdfName, routingKey:
                HelperConstants.RoutingPdf, basicProperties: properties, body: bodyByte);

        }
        public void Publish(string user)
        {
            var channel = _rabbitMQClientService.Connect();

            var bodyString = JsonConvert.SerializeObject(user);
            var bodyByte = Encoding.UTF8.GetBytes(bodyString);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            channel.ExchangeDeclare(HelperConstants.UserExchange, ExchangeType.Fanout, true, false);
            channel.QueueDeclare(queue: HelperConstants.UserQueueName, true, false, false);
            channel.QueueBind(exchange: HelperConstants.UserExchange, queue: HelperConstants.UserQueueName, routingKey: HelperConstants.UserRouting);
            channel.BasicPublish(exchange: HelperConstants.UserExchange, routingKey:
                HelperConstants.UserRouting, basicProperties: properties, body: bodyByte);

        }
        public void LogPublish(ServiceResult<LogDto> service)
        {
            var routingKey = "";
            if (service.Success)
            {
                routingKey = "logs.success";
            }
            else
            {
                routingKey = "logs.error";
            }
            var channel = _rabbitMQClientService.Connect();

            var bodyString = JsonConvert.SerializeObject(service.Data);
            var bodyByte = Encoding.UTF8.GetBytes(bodyString);
            channel.ExchangeDeclare(HelperConstants.LogExchangeName, ExchangeType.Direct, true, false);
            channel.QueueDeclare(queue: HelperConstants.LogQueueName, true, false, false);
            channel.QueueBind(exchange: HelperConstants.LogExchangeName, queue: HelperConstants.LogQueueName, routingKey: routingKey);
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: HelperConstants.LogExchangeName, routingKey:
                            routingKey, basicProperties: properties, body: bodyByte);
        }
    }
}
