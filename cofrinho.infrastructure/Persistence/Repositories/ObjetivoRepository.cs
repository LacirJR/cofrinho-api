using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using cofrinho.core.Abstractions.Repositories;
using cofrinho.core.Entities;

namespace cofrinho.infrastructure.Persistence.Repositories;

public class ObjetivoRepository : RepositoryBase<Objetivo>, IObjetivoRepository
{
    public ObjetivoRepository(CofrinhoDbContext dbContext, ISpecificationEvaluator specificationEvaluator) : base(
        dbContext, specificationEvaluator) { }
}