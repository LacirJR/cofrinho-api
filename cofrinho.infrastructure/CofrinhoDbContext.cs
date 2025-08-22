using cofrinho.core.Entities;
using cofrinho.infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace cofrinho.infrastructure;

public class CofrinhoDbContext : DbContext
{
    private readonly IMediator _mediator;

    public CofrinhoDbContext(DbContextOptions options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker.Entries<BaseEntity>();

        foreach (var entity in entities)
        {
            if (entity.State == EntityState.Added)
                entity.Entity.SetDataCriacao();

            if (entity.State == EntityState.Modified)
                entity.Entity.UpdateData(DateTime.UtcNow);

            await _mediator.DispatchDomainEvents(this);
        }

        return (await base.SaveChangesAsync(true, cancellationToken));
    }
}