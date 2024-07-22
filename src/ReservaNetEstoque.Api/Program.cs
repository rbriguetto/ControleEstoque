using MediatR;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/listaops", (IMediator mediator) => {
    return mediator.Send(new ListaOpsCommand());
});

app.MapGet("/criaop", (IMediator mediator, [FromQuery] string codigo) => {
    var request = new CriaOpCommand(); 
    request.Codigo = codigo;
    
    return mediator.Send(request);
});

app.Run();
