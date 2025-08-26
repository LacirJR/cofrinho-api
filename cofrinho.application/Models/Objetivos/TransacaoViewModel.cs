using cofrinho.core.Entities;
using Mapster;

namespace cofrinho.application.Models.Objetivos;

public class TransacaoViewModel
{
    public Guid Id { get; set; }
    public DinheiroViewModel Valor { get;   set; }
    public string Tipo { get; set; }
    public DateTime DataTransacao { get;  set; }
    
    private static readonly TypeAdapterConfig _config;

    static TransacaoViewModel()
    {
        _config = new TypeAdapterConfig();
        _config.NewConfig<Transacao, TransacaoViewModel>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Valor, src => new DinheiroViewModel(src.Valor.Valor, src.Valor.MoedaEnum))
            .Map(dest => dest.Tipo, src => src.Tipo.ToString())
            .Map(dest => dest.DataTransacao, src => src.DataTransacao);

    }

    public static TransacaoViewModel FromEntity(Transacao objetivo)
        => objetivo.Adapt<TransacaoViewModel>(_config);
    
}