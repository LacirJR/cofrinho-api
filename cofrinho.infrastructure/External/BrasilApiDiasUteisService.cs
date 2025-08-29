using System.Collections.Concurrent;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using cofrinho.core.Abstractions.Services;
using cofrinho.infrastructure.Background.Rendimento;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cofrinho.infrastructure.External;

public class BrasilApiDiasUteisService : IDiasUteisService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RendimentoOptions _options;
    private readonly ILogger<BrasilApiDiasUteisService> _logger;

    private readonly ConcurrentDictionary<int, HashSet<DateOnly>> _cacheFeriados = new();

    public BrasilApiDiasUteisService(IHttpClientFactory httpClientFactory, IOptions<RendimentoOptions> options, ILogger<BrasilApiDiasUteisService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> EhDiaUtil(DateOnly data, CancellationToken cancellationToken = default)
    {
        // Sábado ou domingo
        var diaDaSemana = data.DayOfWeek;
        if (diaDaSemana is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return false;

        var feriados = await ObterFeriadosAsync(data.Year, cancellationToken);
        return !feriados.Contains(data);
    }

    public async Task<DateOnly> ProximoDiaUtil(DateOnly data, CancellationToken cancellationToken = default)
    {
        var d = data.AddDays(1);
        while (!await EhDiaUtil(d, cancellationToken))
        {
            d = d.AddDays(1);
        }
        return d;
    }

    private async Task<HashSet<DateOnly>> ObterFeriadosAsync(int ano, CancellationToken cancellationToken)
    {
        if (_cacheFeriados.TryGetValue(ano, out var set))
            return set;

        try
        {
            var url = $"{_options.FeriadosApiBase.TrimEnd('/')}/{ano}";
            var client = _httpClientFactory.CreateClient("ExternalServicesClient");
            using var response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            var result = new HashSet<DateOnly>();
            foreach (var element in json.RootElement.EnumerateArray())
            {
                if (element.TryGetProperty("date", out var dateProp))
                {
                    if (DateOnly.TryParse(dateProp.GetString(), out var feriado))
                    {
                        result.Add(feriado);
                    }
                }
            }

            _cacheFeriados[ano] = result;
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao consultar feriados na BrasilAPI. Considerando apenas fins de semana para ano {Ano}.", ano);
            var vazio = new HashSet<DateOnly>();
            _cacheFeriados[ano] = vazio;
            return vazio;
        }
    }
}
