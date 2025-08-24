using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace cofrinho.application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMapster();
        
        return services;
    }
}
