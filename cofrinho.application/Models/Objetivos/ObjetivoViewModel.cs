using cofrinho.core.Common.Extensions;
using cofrinho.core.Entities;
using cofrinho.core.ValueObjects;
using Mapster;

namespace cofrinho.application.Models.Objetivos;

public class ObjetivoViewModel
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public string ImagemUrl { get; set; }
    public DinheiroViewModel? ValorAlvo { get; set; }
    public DateTime? Prazo { get; set; }
    public string Status { get; set; }
    public string Categoria { get; set; }
    public string DataCriacao { get; set; }

    private static readonly TypeAdapterConfig _config;

    static ObjetivoViewModel()
    {
        _config = new TypeAdapterConfig();
        _config.NewConfig<Objetivo, ObjetivoViewModel>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Titulo, src => src.Titulo)
            .Map(dest => dest.Descricao, src => src.Descricao ?? string.Empty)
            .Map(dest => dest.ImagemUrl, src => src.ImagemUrl != null ? src.ImagemUrl.Valor : string.Empty)
            .Map(dest => dest.ValorAlvo, src => new DinheiroViewModel(src.ValorAlvo.Valor, src.ValorAlvo.MoedaEnum))
            .Map(dest => dest.Prazo, src => src.Prazo)
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.Categoria, src => src.Categoria.GetDescription())
            .Map(dest => dest.DataCriacao, src => src.DataCriacao);
    }

    public static ObjetivoViewModel FromEntity(Objetivo objetivo)
        => objetivo.Adapt<ObjetivoViewModel>(_config);
    
    public static List<ObjetivoViewModel> FromEntities(IEnumerable<Objetivo> objetivos)
        => objetivos.Adapt<List<ObjetivoViewModel>>(_config);
}