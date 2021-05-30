using System;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace SpecflowEventualConsistency.Infrastructure
{
    public class EventPublisher
    {
        private readonly RabbitMqSettings _settings;
        private IConnection _connection;

        public EventPublisher(IOptions<RabbitMqSettings> settings)
        {
            _settings = settings.Value;
            CreateConnection();
        }
        
        public async Task PublishEvent<TEvent>(TEvent @event)
        {
            if (ConnectionExists())
            {
                using (var channel = _connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _settings.Queue, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var json = JsonConvert.SerializeObject(@event);
                    var body = Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(exchange: "", routingKey: _settings.Queue, basicProperties: null, body: body);
                }
            }
            using var bus = RabbitHutch.CreateBus($"host={_settings.HostName};port={_settings.Port};virtualHost={_settings.VirtualHost};username={_settings.Username};password={_settings.Password}");
            await bus.PubSub.PublishAsync(@event).ConfigureAwait(false);
        }
        
        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _settings.HostName,
                    Port = _settings.Port,
                    UserName = _settings.Username,
                    Password = _settings.Password,
                    VirtualHost = _settings.VirtualHost
                };
                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create connection: {ex.Message}");
            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }

            CreateConnection();

            return _connection != null;
        }
    }
}