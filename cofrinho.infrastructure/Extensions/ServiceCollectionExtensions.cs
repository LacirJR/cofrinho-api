using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace cofrinho.infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<CofrinhoDbContext>(c =>
        {
            
        });
        
        return services;
    }
}