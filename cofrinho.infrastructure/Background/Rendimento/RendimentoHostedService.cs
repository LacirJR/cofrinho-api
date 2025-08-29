using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using cofrinho.core.Abstractions.Services;

namespace cofrinho.infrastructure.Background.Rendimento;

public class RendimentoHostedService : BackgroundService
{
    private readonly ILogger<RendimentoHostedService> _logger;
    private readonly RendimentoOptions _options;
    private readonly IRelogio _relogio;
    private readonly IDiasUteisService _diasUteisService;
    private readonly ITaxaRendimentoProvider _taxaProvider;
    private readonly IRendimentoCalculator _calculator;

    public RendimentoHostedService(
        ILogger<RendimentoHostedService> logger,
        IOptions<RendimentoOptions> options,
        IRelogio relogio,
        IDiasUteisService diasUteisService,
        ITaxaRendimentoProvider taxaProvider,
        IRendimentoCalculator calculator)
    {
        _logger = logger;
        _options = options.Value;
        _relogio = relogio;
        _diasUteisService = diasUteisService;
        _taxaProvider = taxaProvider;
        _calculator = calculator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Habilitado)
        {
            _logger.LogInformation("[Rendimento] Serviço desabilitado por configuração (RENDIMENTO_HABILITADO=false). Não executará.");
            return;
        }

        _logger.LogInformation("[Rendimento] Serviço iniciado. Fuso={Fuso}, HorárioExecucao={Horario}", _options.FusoHorarioId, _options.HorarioExecucao);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var delay = await CalcularDelayParaProximaExecucao(stoppingToken);
                _logger.LogInformation("[Rendimento] Próxima execução em {Delay}.", delay);
                await Task.Delay(delay, stoppingToken);

                await ExecutarCicloAsync(stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Encerrando
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Rendimento] Erro inesperado no loop principal do hosted service.");
                // Pequeno atraso para evitar loop apertado em caso de falhas recorrentes
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        _logger.LogInformation("[Rendimento] Serviço finalizado.");
    }

    private async Task<TimeSpan> CalcularDelayParaProximaExecucao(CancellationToken ct)
    {
        var tz = _relogio.ObterTimeZone(_options.FusoHorarioId);
        var agoraFuso = _relogio.AgoraNoFuso(_options.FusoHorarioId);

        // Monta a data/hora alvo de hoje no fuso
        var dataHoje = DateOnly.FromDateTime(agoraFuso.DateTime);
        var hojeExecLocal = dataHoje.ToDateTime(_options.HorarioExecucao);
        var hojeOffset = tz.GetUtcOffset(hojeExecLocal);
        var hojeExec = new DateTimeOffset(hojeExecLocal, hojeOffset);

        DateOnly dataAlvo = dataHoje;
        if (agoraFuso <= hojeExec)
        {
            // Se ainda não chegou a hora de hoje, verifica se hoje é dia útil
            if (!await _diasUteisService.EhDiaUtil(dataAlvo, ct))
            {
                dataAlvo = await _diasUteisService.ProximoDiaUtil(dataAlvo, ct);
            }
        }
        else
        {
            // Já passou a hora de hoje; agenda para próximo dia útil
            dataAlvo = await _diasUteisService.ProximoDiaUtil(dataHoje, ct);
        }

        var proximaExecLocal = dataAlvo.ToDateTime(_options.HorarioExecucao);
        var proxOffset = tz.GetUtcOffset(proximaExecLocal);
        var proximaExec = new DateTimeOffset(proximaExecLocal, proxOffset);

        var delay = proximaExec - agoraFuso;
        if (delay < TimeSpan.Zero)
            delay = TimeSpan.Zero;

        return delay;
    }

    private async Task ExecutarCicloAsync(CancellationToken ct)
    {
        var agoraFuso = _relogio.AgoraNoFuso(_options.FusoHorarioId);
        var dataRef = DateOnly.FromDateTime(agoraFuso.DateTime);

        // Se por algum motivo a execução caiu num dia não útil, não processa
        if (!await _diasUteisService.EhDiaUtil(dataRef, ct))
        {
            _logger.LogInformation("[Rendimento] Data {Data} não é dia útil. Execução ignorada.", dataRef);
            return;
        }

        // Placeholder de catch-up: percorrer até 'ontem' limitado por LimiteCatchUp
        var dataCursor = dataRef.AddDays(-1);
        var limite = _options.LimiteCatchUp;
        var diasRecuperados = 0;
        while (diasRecuperados < limite && dataCursor < dataRef)
        {
            if (await _diasUteisService.EhDiaUtil(dataCursor, ct))
            {
                _logger.LogInformation("[Rendimento] (Dry-run) Catch-up para {Data}.", dataCursor);
                // Aqui futuramente: processar rendimentos desse dia (DB)
            }
            dataCursor = dataCursor.AddDays(1);
            diasRecuperados++;
        }

        // Processamento do dia corrente (dry-run nesta fase)
        var taxaAnual = await _taxaProvider.ObterTaxaAnualAsync(dataRef, ct);
        var taxaDiaria = _calculator.CalcularTaxaDiaria(taxaAnual);

        _logger.LogInformation("[Rendimento] (Dry-run) Execução em {Data}. TaxaAnual={TaxaAnual:P4}, TaxaDiaria={TaxaDiaria:P6}", dataRef, taxaAnual, taxaDiaria);
        // Próxima etapa: consultar objetivos elegíveis, calcular saldo e persistir transações de rendimento com idempotência
    }
}