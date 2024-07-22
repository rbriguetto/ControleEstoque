using MediatR;

public class CriaOpCommand : IRequest<CriaOpResponse> {
    public string Codigo { get; set; } = "";
}

public class CriaOpResponse { 
    public string CodigoOpCriado { get; set; } = "";
}