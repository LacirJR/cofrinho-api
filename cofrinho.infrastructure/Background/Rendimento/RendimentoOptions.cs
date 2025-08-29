using cofrinho.core.Enums;

namespace cofrinho.infrastructure.Background.Rendimento;

public class RendimentoOptions
{
    public bool Habilitado { get; set; } = false;
    public string FusoHorarioId { get; set; } = "America/Sao_Paulo";
    // Horário de execução diário no fuso configurado (ex.: 02:00)
    public TimeOnly HorarioExecucao { get; set; } = new TimeOnly(2, 0);

    // Fonte da taxa: "Externa" ou "Fixa"
    public FonteTaxaEnum FonteTaxa { get; set; } = FonteTaxaEnum.Externa;
    public decimal TaxaAnualPadrao { get; set; } = 0.1325m; // 13.25% a.a. como fallback

    public int LimiteCatchUp { get; set; } = 5;
    public int TamanhoPagina { get; set; } = 500;

    // Integrações externas
    public string FeriadosApiBase { get; set; } = "https://brasilapi.com.br/api/feriados/v1";
    public string? CdiApiBase { get; set; } // opcional; se nulo, usa TaxaAnualPadrao
}
