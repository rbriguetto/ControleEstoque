using MediatR;

public class RealizarTransferenciaCommand : IRequest<bool> {

    public string NotaFiscalOrigem { get; set; } = string.Empty;
    public string FilialDestino { get; set; } = string.Empty;
    public int Quantidade { get; set; } = 0;
}
