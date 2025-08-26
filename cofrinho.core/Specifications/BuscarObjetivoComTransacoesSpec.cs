using Ardalis.Specification;
using cofrinho.core.Entities;

namespace cofrinho.core.Specifications;

public class BuscarObjetivoComTransacoesSpec : SingleResultSpecification<Objetivo>
{
    public BuscarObjetivoComTransacoesSpec(Guid id)
    {
        Query.AsNoTracking()
            .Include(x => x.Transacoes)
            .Where(x => x.Id == id);
    }
}