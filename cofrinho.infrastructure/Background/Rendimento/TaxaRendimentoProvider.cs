using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using cofrinho.core.Abstractions.Services;
using cofrinho.core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cofrinho.infrastructure.Background.Rendimento;

public class TaxaRendimentoProvider : ITaxaRendimentoProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RendimentoOptions _options;
    private readonly ILogger<TaxaRendimentoProvider> _logger;

    public TaxaRendimentoProvider(IHttpClientFactory httpClientFactory, IOptions<RendimentoOptions> options, ILogger<TaxaRendimentoProvider> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<decimal> ObterTaxaAnualAsync(DateOnly data, CancellationToken cancellationToken = default)
    {
        // Fallback imediato para taxa fixa se fonte não for externa ou base não definida
        if (_options.FonteTaxa == FonteTaxaEnum.Fixa || string.IsNullOrWhiteSpace(_options.CdiApiBase))
        {
            return _options.TaxaAnualPadrao;
        }

        try
        {
            // Convenção genérica: GET {base}?date=yyyy-MM-dd
            var url = $"{_options.CdiApiBase}{(_options.CdiApiBase!.Contains("?") ? "&" : "?")}date={data:yyyy-MM-dd}";
            var client = _httpClientFactory.CreateClient("ExternalServicesClient");
            using var response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            var root = json.RootElement;

            // Tentar achar um campo numérico com taxa anual (ex.: "rate", "valor", "cdiAnual")
            if (TryLerDecimal(root, out var taxa))
            {
                return taxa;
            }

            _logger.LogWarning("Não foi possível interpretar a taxa anual do payload da API CDI. Usando fallback TaxaAnualPadrao.");
            return _options.TaxaAnualPadrao;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha ao consultar taxa de rendimento externa. Usando TaxaAnualPadrao.");
            return _options.TaxaAnualPadrao;
        }
    }

    private static bool TryLerDecimal(JsonElement root, out decimal taxa)
    {
        // Caso seja um valor simples
        if (root.ValueKind == JsonValueKind.Number && root.TryGetDecimal(out taxa))
            return true;

        // Caso seja objeto com propriedades
        if (root.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in root.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.Number && prop.Value.TryGetDecimal(out taxa))
                    return true;
                if (prop.Value.ValueKind == JsonValueKind.String)
                {
                    var s = prop.Value.GetString();
                    if (decimal.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out taxa))
                        return true;
                }
            }
        }

        // Caso seja array, tenta primeira célula numérica
        if (root.ValueKind == JsonValueKind.Array)
        {
            foreach (var el in root.EnumerateArray())
            {
                if (el.ValueKind == JsonValueKind.Number && el.TryGetDecimal(out taxa))
                    return true;
                if (el.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in el.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.Number && prop.Value.TryGetDecimal(out taxa))
                            return true;
                    }
                }
            }
        }

        taxa = 0m;
        return false;
    }
}