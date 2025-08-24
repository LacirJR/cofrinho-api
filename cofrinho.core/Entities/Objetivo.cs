using cofrinho.core.Enums;
using cofrinho.core.ValueObjects;
using Flunt.Notifications;
using Flunt.Validations;

namespace cofrinho.core.Entities;

public class Objetivo : BaseEntity
{
    protected Objetivo()
    {
    }

    private Objetivo(string titulo, string? descricao, Dinheiro valorAlvo, DateTime? prazo,
        StatusObjetivoEnum status, CategoriaEnum categoria)
    {
        Id = Guid.NewGuid();
        Titulo = titulo;
        Descricao = descricao;
        ValorAlvo = valorAlvo;
        Prazo = prazo;
        Status = status;
        Categoria = categoria;

        AddNotifications(ValorAlvo.Notifications);
        Validar();
    }

    public Guid Id { get; private set; }
    public string Titulo { get; private set; }
    public string? Descricao { get; private set; }

    public Url? ImagemUrl { get; private set; }
    public Dinheiro ValorAlvo { get; private set; }
    public DateTime? Prazo { get; private set; }
    public StatusObjetivoEnum Status { get; private set; }
    public CategoriaEnum Categoria { get; private set; }

    private List<Transacao> _transacoes = new();

    public IReadOnlyCollection<Transacao> Transacoes => _transacoes.AsReadOnly();

    private void Validar()
    {
        var contrato = new Contract<Objetivo>()
            .Requires()
            .IsNotNullOrWhiteSpace(Titulo, nameof(Titulo), "Titulo não pode ser nulo.")
            .IsNotNull(Status, nameof(Status), "Status não pode ser nulo.")
            .IsNotNull(Categoria, nameof(Categoria), "Categoria não pode ser nula.");

        AddNotifications(contrato);
    }

    public static Objetivo Criar(string titulo, string? descricao, decimal? valorAlvo, TipoMoedaEnum? tipoMoeda,
        DateTime? prazo, CategoriaEnum categoria)
    {
        if (valorAlvo is not null && tipoMoeda is null)
            tipoMoeda = TipoMoedaEnum.BRL;

        if (valorAlvo is null)
        {
            valorAlvo = 0;
            tipoMoeda = TipoMoedaEnum.BRL;
        }


        var objetivo = new Objetivo(titulo, descricao, Dinheiro.Criar(valorAlvo.Value, tipoMoeda.Value), prazo,
            StatusObjetivoEnum.EmProgresso, categoria);
        return objetivo;
    }

    public void AlterarTitulo(string titulo)
    {
        if (titulo.Length < 3)
            AddNotification(nameof(Titulo), "Título deve ter pelo menos 3 caracteres.");
        else
            Titulo = titulo;
    }

    public void AlterarDescricao(string descricao)
    {
        if (descricao.Length < 3)
            AddNotification(nameof(Descricao), "Título deve ter pelo menos 3 caracteres.");
        else
            Descricao = descricao;
    }

    public void AlterarValorAlvo(decimal valorAlvo, TipoMoedaEnum moedaEnum)
    {
        ValorAlvo = Dinheiro.Criar(valorAlvo, moedaEnum);
        AddNotifications(ValorAlvo.Notifications);
    }

    public void AlterarPrazo(DateTime prazo)
    {
        Prazo = prazo;

        if (Prazo.Value.Date < DateTime.UtcNow.Date)
            AddNotification(nameof(Prazo), "Prazo deve ser maior que a data atual.");
    }

    public void AlterarStatus(StatusObjetivoEnum status)
    {
        if (Status == StatusObjetivoEnum.Cancelado)
            AddNotification(nameof(Status), "Não pode alterar status de um objetivo cancelado.");
        
        Status = status;
    }

    public void Deletar()
    {
        MarkAsDeleted();
    }
}