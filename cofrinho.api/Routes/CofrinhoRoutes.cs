using cofrinho.application.Commands.Objetivos.Criar;
using cofrinho.application.Commands.Objetivos.Editar;
using cofrinho.application.Commands.Objetivos.Remover;
using cofrinho.application.Commands.Transacoes.Criar;
using cofrinho.application.Models;
using cofrinho.application.Models.Objetivos;
using cofrinho.application.Queries.Objetivos.BuscarPorId;
using cofrinho.application.Queries.Objetivos.Listar;
using MediatR;
using Microsoft.OpenApi.Models;
using Sprache;

namespace cofrinho_api.Routes;

public static class CofrinhoRoutes
{
    public static WebApplication UseRoutes(this WebApplication builder)
    {
        var routeGroup = builder.MapGroup("api/objetivo").WithName("Objetivo").WithSummary("Objetivo")
            .WithDisplayName("Objetivo");

        routeGroup.MapGet("/", async (ISender sender, [AsParameters] ListarObjetivosQuery request) =>
            {
                var result = await sender.Send(request);

                if (result.IsSuccess)
                {
                    if (result.Data.TotalItems > 0)
                        return Results.Ok(result);

                    return Results.NoContent();
                }

                return Results.BadRequest();
            }).WithName("GetObjetivos")
            .WithSummary("Listar os Objetivos")
            .WithDescription("Listar os objetivos ativos paginados")
            .Produces<ResultViewModel<PagedResult<ObjetivoViewModel>>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent);

        routeGroup.MapPost("/", async (ISender sender, CriarObjetivoCommand command) =>
            {
                var result = await sender.Send(command);

                if (result.IsSuccess)
                    return Results.Created("/", result);

                return Results.BadRequest(result);
            }).WithName("CriarObjetivo")
            .WithSummary("Criar um Objetivo")
            .Produces<ResultViewModel<Guid>>(StatusCodes.Status201Created)
            .Produces<ResultViewModel>(StatusCodes.Status400BadRequest);


        routeGroup.MapGet("/{id:guid}", async (ISender sender, Guid id) =>
            {
                var command = new BuscarObjetivoPorIdCommand(id);

                var result = await sender.Send(command);

                if (result.IsSuccess)
                    return Results.Ok(result);

                return Results.BadRequest(result);
            }).WithName("ObterObjetivo")
            .WithSummary("Obter um Objetivo")
            .WithDescription("Obter um Objetivo com Transações")
            .Produces<ResultViewModel<ObjetivoComTransacoesViewModel>>(StatusCodes.Status200OK)
            .Produces<ResultViewModel>(StatusCodes.Status400BadRequest);

        
        routeGroup.MapPut("/{id:guid}", async (ISender sender, Guid id, EditarObjetivoByIdCommand request) =>
            {
                var command = request with { Id = id };

                var result = await sender.Send(command);

                if (result.IsSuccess)
                    return Results.Ok(result);

                return Results.BadRequest(result);
            }).WithName("EditarObjetivo")
            .WithSummary("Editar um Objetivo")
            .Produces<ResultViewModel>(StatusCodes.Status200OK)
            .Produces<ResultViewModel>(StatusCodes.Status400BadRequest);

        routeGroup.MapDelete("/{id:guid}", async (ISender sender, Guid id) =>
            {
                var command = new RemoverObjetivoCommand(id);

                var result = await sender.Send(command);

                if (result.IsSuccess)
                    return Results.Ok(result);

                return Results.BadRequest(result);
            }).WithName("RemoverObjetivo")
            .WithSummary("Remover um Objetivo")
            .Produces<ResultViewModel>(StatusCodes.Status200OK)
            .Produces<ResultViewModel>(StatusCodes.Status400BadRequest);



       var transacaoGroup = routeGroup.MapGroup("{objetivoId:guid}/transacoes").WithName("Transação").WithSummary("Transação")
            .WithDisplayName("Transação");
        
        
        transacaoGroup.MapPost("/", async (ISender sender, Guid objetivoId, CriarTransacoesCommand request) =>
        {
            var command = request with {ObjetivoId = objetivoId};
            var result = await sender.Send(command);
            
            if(result.IsSuccess)
                return Results.Created("/", result);
            
            return Results.BadRequest(result);
        }).WithName("CriarTransacao")
        .WithSummary("Criar uma Transação")
        .Produces<ResultViewModel<Guid>>(StatusCodes.Status201Created)
        .Produces<ResultViewModel>(StatusCodes.Status400BadRequest);
        
        return builder;
    }
}