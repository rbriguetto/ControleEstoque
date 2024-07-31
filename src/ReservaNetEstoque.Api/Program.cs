using Infraestructure.Elasticsearch.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ReservaNetEstoque.Api.Commands;
using ReservaNetEstoque.Api.Workers;
using Serilog;

try {

    var builder = WebApplication.CreateBuilder(args);

    // builder.AddSerilog(builder.Configuration, "ReservaNetEstoque.Api", "ReservaNetEstoque:");

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

    builder.Services.AddHostedService<ConsomeFileGerarTransferenciaWorker>();
    builder.Services.AddHostedService<VerificaNotasFiscaisPendentesPriorizacaoWorker>();

    var app = builder.Build();


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapGet("/priorizacao/prioriza-nota", (IMediator mediator, [FromQuery] string notaFiscal) => {
        return mediator.Send(new GeraTransferenciaASerRealizadaCommand() { NotaFiscal = notaFiscal });
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}

