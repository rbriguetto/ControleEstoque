using MediatR;

namespace ReservaNetEstoque.Api.Commands;

public class GeraTransferenciaASerRealizadaCommand : IRequest<bool> {
    public string NotaFiscal { get; set; } = string.Empty;
}
