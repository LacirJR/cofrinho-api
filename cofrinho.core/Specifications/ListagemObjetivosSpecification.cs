using Ardalis.Specification;
using cofrinho.core.Entities;

namespace cofrinho.core.Specifications;

public class ListagemObjetivosSpecification : Specification<Objetivo>
{
    public ListagemObjetivosSpecification()
    {
        Query.AsNoTracking()
            .Where(x => !x.EstaDeletado);
        
    }
}