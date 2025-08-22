using cofrinho.core.Enums;

namespace cofrinho.core.Entities;

public class Objetivo : BaseEntity
{
    public Guid Id { get; private set; }
    public string Titulo { get; private set; }
    public string? Descricao { get; private set; }
    public string? ImagemUrl { get; private set; }
    public decimal QuantidadeAlvo { get; private set; }
    public DateTime? Prazo { get; private set; }
    public StatusObjetivoEnum Status { get; private set; }
    
    private List<Transacao> _transacoes = new();

    public IReadOnlyCollection<Transacao> Transacoes => _transacoes.AsReadOnly();
}