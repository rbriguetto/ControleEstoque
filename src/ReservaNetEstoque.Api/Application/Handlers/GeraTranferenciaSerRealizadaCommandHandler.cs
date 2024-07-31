using MediatR;
using ReservaNetEstoque.Api.Commands;

namespace ReservaNetEstoque.Api.Handlers;

public class GeraTransferenciaASerRealizadaCommandHandler : IRequestHandler<GeraTransferenciaASerRealizadaCommand, bool>
{
    private readonly ILogger<GeraTransferenciaASerRealizadaCommandHandler> _logger;

    public GeraTransferenciaASerRealizadaCommandHandler(ILogger<GeraTransferenciaASerRealizadaCommandHandler> logger)
    {
        _logger = logger;
    }

    public Task<bool> Handle(GeraTransferenciaASerRealizadaCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"CHEGOU NO HANDLER: {request.NotaFiscal}");
        return Task.FromResult(true);
    }
}