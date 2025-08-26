using cofrinho.core.Entities;
using cofrinho.core.Enums;
using Mapster;

namespace cofrinho.application.Models.Objetivos;


public class ObjetivoComTransacoesViewModel : ObjetivoViewModel
{
    public List<TransacaoViewModel> Transacoes { get; set; } = new();
    public DinheiroViewModel TotalDepositos { get; set; }
    public DinheiroViewModel TotalSaques { get; set; }
    public DinheiroViewModel Total { get; set; }
    public DinheiroViewModel FaltaParaAlvo { get; set; }

    private static readonly TypeAdapterConfig _config;

    static ObjetivoComTransacoesViewModel()
    {
        _config = new TypeAdapterConfig();
        _config.NewConfig<Objetivo, ObjetivoComTransacoesViewModel>()
            .Map(dest => dest.Transacoes, src => (src.Transacoes.OrderByDescending(x => x.DataTransacao).ToList() ?? Enumerable.Empty<Transacao>())
                .Select(TransacaoViewModel.FromEntity).ToList())
            .AfterMapping((src, dest) =>
            {
                var trans = src.Transacoes ?? Enumerable.Empty<Transacao>();
                var moeda = src.ValorAlvo.MoedaEnum;

                var depositos = trans.Where(t => t.Tipo == TipoTransacaoEnum.Deposito)
                    .Select(t => t.Valor?.Valor ?? 0m)
                    .DefaultIfEmpty(0m)
                    .Sum();

                var saques = trans.Where(t => t.Tipo == TipoTransacaoEnum.Saque)
                    .Select(t => t.Valor?.Valor ?? 0m)
                    .DefaultIfEmpty(0m)
                    .Sum();
                
                

                dest.TotalDepositos = new DinheiroViewModel(depositos, moeda);
                dest.TotalSaques    = new DinheiroViewModel(saques, moeda);
                dest.Total          = new DinheiroViewModel(depositos - saques, moeda);
                
                dest.FaltaParaAlvo = new DinheiroViewModel(src.ValorAlvo.Valor - dest.Total.Valor, moeda);
            });
    }

    public static ObjetivoComTransacoesViewModel FromEntity(Objetivo objetivo)
        => objetivo.Adapt<ObjetivoComTransacoesViewModel>(_config);
}