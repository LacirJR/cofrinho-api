using System.Collections;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using cofrinho_api.Routes;
using cofrinho_api.SchemaFilters;
using cofrinho.application;
using cofrinho.application.Extensions;
using cofrinho.core.Enums;
using cofrinho.infrastructure.Extensions;
using cofrinho.infrastructure.Persistence;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateSlimBuilder(args);

Env.Load();

foreach (DictionaryEntry envVar in Environment.GetEnvironmentVariables())
{
    builder.Configuration[envVar.Key.ToString()] = envVar.Value?.ToString();
}


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Cofrinho API",
        Version = "v1",
        Description = "API para simulação do cofrinho, baseada no CDI."
    });
    c.SchemaFilter<EnumAsStringSchemaFilter>();

});

builder.Services.Configure<JsonOptions>(opt =>
{
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    opt.JsonSerializerOptions.TypeInfoResolverChain.Add(new DefaultJsonTypeInfoResolver());
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, new DefaultJsonTypeInfoResolver());
    options.SerializerOptions.TypeInfoResolverChain.Add(new DefaultJsonTypeInfoResolver());
});

builder.Services.AddMediatR(cfg =>
{
    cfg.LicenseKey = builder.Configuration["LICENSE_MEDIATR"];
    cfg.RegisterServicesFromAssemblies(
        typeof(AssemblyReference).Assembly,
        typeof(Program).Assembly            
    );
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await app.MigrateDatabaseAsync();


if (app.Environment.IsDevelopment())
{
    
    app.UseDeveloperExceptionPage(); // Em apps recentes já é automático com o template
}
else
{
    app.UseExceptionHandler("/error"); // Página/endpoint de erro em produção
}

app.MapSwagger("/openapi/{documentName}.json");
app.MapScalarApiReference(options =>
{
    options.WithTitle("Cofrinho API - Doc")
        .WithSidebar()
        .WithDarkMode()
        .WithDefaultOpenAllTags();
});

app.UseRoutes();

app.Run();