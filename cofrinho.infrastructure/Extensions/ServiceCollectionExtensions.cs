using System.Reflection;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using cofrinho.core.Abstractions;
using cofrinho.core.Abstractions.Repositories;
using cofrinho.infrastructure.Persistence;
using cofrinho.infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace cofrinho.infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ISpecificationEvaluator>(_ => SpecificationEvaluator.Default);
        

        services.AddDbContext<CofrinhoDbContext>(options =>
        {
            options.UseNpgsql(configuration["DB_CONNECTION"],
                o =>
                {
                    o.MigrationsAssembly(typeof(CofrinhoDbContext).Assembly.FullName);
                    o.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                });
        });

        services.AddScoped<IObjetivoRepository, ObjetivoRepository>();
        services.AddScoped<ITransacaoRepository, TransacaoRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}