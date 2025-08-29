using System.Globalization;
using System.Net.Http;
using System.Reflection;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using cofrinho.core.Abstractions;
using cofrinho.core.Abstractions.Repositories;
using cofrinho.core.Abstractions.Services;
using cofrinho.core.Enums;
using cofrinho.infrastructure.Background.Rendimento;
using cofrinho.infrastructure.External;
using cofrinho.infrastructure.Persistence;
using cofrinho.infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

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

        // HTTP Client para integrações externas com política de resiliência
        services.AddHttpClient("ExternalServicesClient")
            .AddPolicyHandler(GetRetryPolicy());

        // Options de Rendimento a partir de variáveis de ambiente/configuração
        services.AddOptions<RendimentoOptions>().Configure<IConfiguration>((opt, cfg) =>
        {
            if (bool.TryParse(cfg["RENDIMENTO_HABILITADO"], out var habilitado)) opt.Habilitado = habilitado;
            opt.FusoHorarioId = cfg["RENDIMENTO_FUSO"] ?? opt.FusoHorarioId;
            if (TimeOnly.TryParse(cfg["RENDIMENTO_HORARIO_EXECUCAO"], CultureInfo.InvariantCulture, out var hora)) opt.HorarioExecucao = hora;
            if (Enum.TryParse<FonteTaxaEnum>(cfg["RENDIMENTO_FONTE_TAXA"], true, out var fonteTaxa)) opt.FonteTaxa = fonteTaxa;
            if (decimal.TryParse(cfg["RENDIMENTO_TAXA_ANUAL_PADRAO"], NumberStyles.Float, CultureInfo.InvariantCulture, out var taxaAnual)) opt.TaxaAnualPadrao = taxaAnual;
            if (int.TryParse(cfg["RENDIMENTO_LIMITE_CATCHUP"], out var catchup)) opt.LimiteCatchUp = catchup;
            if (int.TryParse(cfg["RENDIMENTO_TAMANHO_PAGINA"], out var page)) opt.TamanhoPagina = page;
            opt.FeriadosApiBase = cfg["FERIADOS_API_BASE"] ?? opt.FeriadosApiBase;
            opt.CdiApiBase = cfg["CDI_API_BASE"] ?? opt.CdiApiBase;
        });

        // Serviços auxiliares para rendimento
        services.AddSingleton<IRelogio, RelogioSistema>();
        services.AddSingleton<IDiasUteisService, BrasilApiDiasUteisService>();
        services.AddSingleton<IRendimentoCalculator, RendimentoCalculator>();
        services.AddSingleton<ITaxaRendimentoProvider, TaxaRendimentoProvider>();

        // HostedService condicionado à flag
        var flagHabilitado = false;
        if (bool.TryParse(configuration["RENDIMENTO_HABILITADO"], out var f)) flagHabilitado = f;
        if (flagHabilitado)
        {
            services.AddHostedService<RendimentoHostedService>();
        }

        return services;
    }
    
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError() // Trata erros de rede, 5XX e 408 (Request Timeout)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}