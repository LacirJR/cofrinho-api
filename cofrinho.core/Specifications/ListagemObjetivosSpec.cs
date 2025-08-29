using Ardalis.Specification;
using cofrinho.core.Entities;

namespace cofrinho.core.Specifications;

public class ListagemObjetivosSpec : Specification<Objetivo>
{
    public ListagemObjetivosSpec()
    {
        Query.AsNoTracking()
            .Where(x => !x.EstaDeletado);
        
    }
}