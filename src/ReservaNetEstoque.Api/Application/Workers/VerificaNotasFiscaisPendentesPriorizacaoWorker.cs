
using System.Text;
using System.Text.Json;
using MediatR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ReservaNetEstoque.Api.Commands;

namespace ReservaNetEstoque.Api.Workers;

public class VerificaNotasFiscaisPendentesPriorizacaoWorker : BackgroundService
{
    private readonly ILogger<VerificaNotasFiscaisPendentesPriorizacaoWorker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    
    private const string NOVA_NOTA_PARA_PRIORIZAR = "NOVA_NOTA_PARA_PRIORIZAR";
    private const string GERA_TRANSFERENCIA_A_SER_REALIZADA = "GERA_TRANSFERENCIA_A_SER_REALIZADA";

    public VerificaNotasFiscaisPendentesPriorizacaoWorker(
        ILogger<VerificaNotasFiscaisPendentesPriorizacaoWorker> logger, 
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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


        int i = 0;
        while (!stoppingToken.IsCancellationRequested) 
        {
            var command = new GeraTransferenciaASerRealizadaCommand() { NotaFiscal = i.ToString() };
            var body = JsonSerializer.Serialize(command);
            var bytes = Encoding.UTF8.GetBytes(body);
            channel.BasicPublish(NOVA_NOTA_PARA_PRIORIZAR, string.Empty, null, bytes);
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            i++;
        }
    }
}