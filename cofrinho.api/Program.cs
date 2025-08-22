using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using cofrinho_api.Routes;
using Scalar.AspNetCore;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Cofrinho API",
        Version = "v1",
        Description = "API para simulação do cofrinho, baseada no CDI."
    });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, new DefaultJsonTypeInfoResolver());
});

var app = builder.Build();

app.MapSwagger("/openapi/{documentName}.json");
app.MapScalarApiReference(options =>
{
    options.WithTitle("Cofrinho API - Doc")
        .WithSidebar()
        .WithDarkMode()
        .WithDefaultOpenAllTags();
});

app.UseObjetivoRoutes();

app.Run();