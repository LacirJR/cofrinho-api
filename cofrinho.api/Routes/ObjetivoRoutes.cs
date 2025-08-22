using Microsoft.OpenApi.Models;

namespace cofrinho_api.Routes;

public static class ObjetivoRoutes
{
    public static WebApplication UseObjetivoRoutes(this WebApplication builder)
    {
        var routeGroup = builder.MapGroup("api/objetivo").WithName("Objetivo").WithSummary("Objetivo")
            .WithDisplayName("Objetivo");

        routeGroup.MapGet("/", () =>
        {
            return Results.Ok("Hello World");
        }).WithName("GetObjetivos")
        .WithSummary("Listar os Objetivos")
        .Produces<string>(StatusCodes.Status200OK);
        
        return builder;
    }
}