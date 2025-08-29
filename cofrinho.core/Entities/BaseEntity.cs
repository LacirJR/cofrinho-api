using System.ComponentModel.DataAnnotations.Schema;
using Flunt.Notifications;
using MediatR;

namespace cofrinho.core.Entities;

public abstract class BaseEntity : Notifiable<Notification>, IEquatable<BaseEntity>
{
    
    public DateTime DataCriacao { get; private set; }
    public DateTime DataAtualizacao { get; private set; }
    public bool EstaDeletado { get; private set; }

    private readonly List<INotification> _domainEvents = new();

    [NotMapped] public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    public void MarkAsDeleted() => EstaDeletado = true;
    
    public void UpdateData(DateTime dataAtualizacao) => DataAtualizacao = dataAtualizacao;
    
    public void SetDataCriacao() => DataCriacao = DateTime.UtcNow;

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
    
    
    
    public bool Equals(BaseEntity? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return _domainEvents.Equals(other._domainEvents) && DataCriacao.Equals(other.DataCriacao) &&
               DataAtualizacao.Equals(other.DataAtualizacao) && EstaDeletado == other.EstaDeletado;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((BaseEntity)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_domainEvents, DataCriacao, DataAtualizacao, EstaDeletado);
    }
}