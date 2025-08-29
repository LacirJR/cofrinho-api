using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using cofrinho.core.Abstractions;
using cofrinho.core.Abstractions.Repositories;

namespace cofrinho.infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly CofrinhoDbContext _context;

    public UnitOfWork(CofrinhoDbContext context, IObjetivoRepository objetivoRepository, ITransacaoRepository transacaoRepository)
    {
        _context = context;
        Objetivos = objetivoRepository;
        Transacoes = transacaoRepository;
    }


    public IObjetivoRepository Objetivos { get; }
    public ITransacaoRepository Transacoes { get; }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }
}