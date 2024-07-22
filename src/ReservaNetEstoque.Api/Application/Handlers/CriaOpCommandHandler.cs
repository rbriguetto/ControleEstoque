using MediatR;

public class CriaOpCommandHandler : IRequestHandler<CriaOpCommand, CriaOpResponse>
{
    public Task<CriaOpResponse> Handle(CriaOpCommand request, CancellationToken cancellationToken)
    {
        var retorno = new CriaOpResponse() { CodigoOpCriado = "djaiosdjasio" };
        return Task.FromResult(retorno);
    }
}