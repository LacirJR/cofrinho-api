using cofrinho.core.Enums;

namespace cofrinho.core.Entities;

public class Transacao : BaseEntity
{
    public Guid Id { get; private set; }
    public decimal Valor { get;  private set; }
    public TipoTransacaoEnum Tipo { get; private set; }
    public DateTime DataTransacao { get;  private set; }
    public Guid ObjetivoId { get; private set; }
    
    public Objetivo Objetivo { get; private set; }
}