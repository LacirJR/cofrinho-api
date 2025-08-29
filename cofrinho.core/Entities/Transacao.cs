using cofrinho.core.Enums;
using cofrinho.core.ValueObjects;
using Flunt.Validations;

namespace cofrinho.core.Entities;

public class Transacao : BaseEntity
{
    protected Transacao() { }

    private Transacao(Dinheiro valor, TipoTransacaoEnum tipo, DateTime dataTransacao, Guid objetivoId)
    {
        Id = Guid.NewGuid();
        Valor = valor;
        Tipo = tipo;
        DataTransacao = dataTransacao;
        ObjetivoId = objetivoId;
        
        AddNotifications(valor.Notifications);
        Validar();
    }
    public Guid Id { get; private set; }
    public Dinheiro Valor { get;  private set; }
    public TipoTransacaoEnum Tipo { get; private set; }
    public DateTime DataTransacao { get;  private set; }
    public Guid ObjetivoId { get; private set; }
    public Objetivo Objetivo { get; private set; }

    private void Validar()
    {
        var contrato = new Contract<Transacao>()
            .Requires()
            .IsNotNull(Valor, nameof(Valor), "Valor não pode ser nulo.")
            .IsNotNull(Tipo, nameof(Tipo), "Tipo de transação não pode ser nulo.")
            .IsNotNull(DataTransacao, nameof(DataTransacao), "Tipo de transação não pode ser nulo.")
            .IsNotNull(ObjetivoId, nameof(ObjetivoId), "Objetivo Id não pode ser nulo.");
        
        AddNotifications(contrato);
    }

    public static Transacao Criar(Dinheiro valor, TipoTransacaoEnum tipo, DateTime dataTransacao, Guid objetivoId)
    {
        var transacao = new Transacao(valor, tipo, dataTransacao, objetivoId);
        return transacao;
    }
    
    
    
}