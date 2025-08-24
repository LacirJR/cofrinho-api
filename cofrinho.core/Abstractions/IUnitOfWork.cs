using Ardalis.Specification;
using cofrinho.core.Abstractions.Repositories;

namespace cofrinho.core.Abstractions;

public interface IUnitOfWork
{
    IObjetivoRepository Objetivos { get; }
    ITransacaoRepository Transacoes { get; }
    
    Task<int> CommitAsync();
}