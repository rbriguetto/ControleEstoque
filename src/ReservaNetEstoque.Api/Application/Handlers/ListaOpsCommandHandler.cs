using MediatR;

public class ListaOpsCommandHandler : IRequestHandler<ListaOpsCommand, string[]>
{
    public Task<string[]> Handle(ListaOpsCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new string[] { "aaaaaaaaaaaaa", "bbbbbbbbbbbbbbb"});
    }
}