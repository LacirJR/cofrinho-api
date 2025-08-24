using cofrinho.application.Commands.Objetivos.Criar;
using cofrinho.application.Commands.Objetivos.Editar;
using cofrinho.application.Commands.Objetivos.Remover;
using cofrinho.application.Models;
using cofrinho.application.Queries.Objetivos.Listar;
using MediatR;
using Microsoft.OpenApi.Models;
using Sprache;

namespace cofrinho_api.Routes;

public static class ObjetivoRoutes
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
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status201Created);

        routeGroup.MapPost("/", async (ISender sender, CriarObjetivoCommand command) =>
            {
                var result = await sender.Send(command);

                if (result.IsSuccess)
                    return Results.Created("/", result);

                return Results.BadRequest(result);
            }).WithName("CriarObjetivo")
            .WithSummary("Criar um Objetivo")
            .Produces<ResultViewModel>(StatusCodes.Status201Created)
            .Produces<ResultViewModel>(StatusCodes.Status400BadRequest);


        routeGroup.MapPut("/{id}", async (ISender sender, Guid id, EditarObjetivoByIdCommand request) =>
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

        routeGroup.MapDelete("/{id}", async (ISender sender, Guid id) =>
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


        return builder;
    }
}