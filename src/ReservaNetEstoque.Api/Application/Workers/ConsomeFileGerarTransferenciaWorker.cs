
using System.Text;
using System.Text.Json;
using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ReservaNetEstoque.Api.Commands;

namespace ReservaNetEstoque.Api.Workers;

public class ConsomeFileGerarTransferenciaWorker : BackgroundService
{
    private readonly ILogger<ConsomeFileGerarTransferenciaWorker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    
    private const string NOVA_NOTA_PARA_PRIORIZAR = "NOVA_NOTA_PARA_PRIORIZAR";
    private const string GERA_TRANSFERENCIA_A_SER_REALIZADA = "GERA_TRANSFERENCIA_A_SER_REALIZADA";

    public ConsomeFileGerarTransferenciaWorker(
        ILogger<ConsomeFileGerarTransferenciaWorker> logger, 
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested) 
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(_configuration["RABBITMQ"] ?? "amqp://test:test@localhost:5672") ,
                DispatchConsumersAsync = true
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.BasicQos(0, 1, true);

            channel.ExchangeDeclare(exchange: NOVA_NOTA_PARA_PRIORIZAR, type: "fanout", durable: true, autoDelete: false);
            channel.QueueDeclare(queue: GERA_TRANSFERENCIA_A_SER_REALIZADA, durable: true, exclusive: false, autoDelete: false);
            channel.QueueBind(GERA_TRANSFERENCIA_A_SER_REALIZADA, NOVA_NOTA_PARA_PRIORIZAR, string.Empty);

            var consumer = new AsyncEventingBasicConsumer(channel);
            
            consumer.Received += async (model, ea) => { 
                var payload = Encoding.UTF8.GetString(ea.Body.ToArray());
                var command = JsonSerializer.Deserialize<GeraTransferenciaASerRealizadaCommand>(payload);
                using var scope = _serviceProvider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(command);
                channel.BasicAck(ea.DeliveryTag, true);
            };

            channel.BasicConsume(
                queue: GERA_TRANSFERENCIA_A_SER_REALIZADA,
                autoAck: false,
                consumer: consumer
            );

            _logger.LogInformation("Worker plugado na fila");

            await Task.Delay(-1, stoppingToken);
        }
    }
}