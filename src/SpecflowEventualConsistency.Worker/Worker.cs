using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SpecflowEventualConsistency.Application;
using SpecflowEventualConsistency.Domain;
using SpecflowEventualConsistency.Infrastructure;

namespace SpecflowEventualConsistency.Worker
{
    public class Worker : BackgroundService
    {
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Worker> _logger;
        private IConnection _connection;
        private IModel _channel;

        public Worker(IOptions<RabbitMqSettings> rabbitMqSettings, IServiceProvider serviceProvider, ILogger<Worker> logger)
        {
            _rabbitMqSettings = rabbitMqSettings.Value;
            _serviceProvider = serviceProvider;
            _logger = logger;

            InitializeRabbitMqListener();
        }
        
        private void InitializeRabbitMqListener()
        {
            _logger.LogInformation($"SETTINGS: {_rabbitMqSettings.HostName}:{_rabbitMqSettings.Port}");
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqSettings.HostName,
                Port = _rabbitMqSettings.Port,
                UserName = _rabbitMqSettings.Username,
                Password = _rabbitMqSettings.Password,
                VirtualHost = _rabbitMqSettings.VirtualHost
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _rabbitMqSettings.Queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var newOrder = JsonConvert.DeserializeObject<NewOrderEvent>(content);

                    await HandleMessage(newOrder);

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception exception)
                {
                   _logger.LogError(exception.Message, exception); 
                }
                
            };

            _channel.BasicConsume(_rabbitMqSettings.Queue, false, consumer);

            return Task.CompletedTask;
        }
        
        private async Task HandleMessage(NewOrderEvent newOrderEvent)
        {
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Publish(new ProcessOrderCommand.Command(newOrderEvent.Order)).ConfigureAwait(false);
            _logger.LogInformation($"New Order Received: CustomerId: {newOrderEvent.Order.CustomerId}, ProductId: {newOrderEvent.Order.ProductId}, Amount: {newOrderEvent.Order.Amount}");
        }
    }
}