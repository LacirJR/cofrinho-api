using cofrinho.core.Entities;
using cofrinho.infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace cofrinho.infrastructure.Persistence;


public class CofrinhoDbContext : DbContext
{
    private readonly IMediator _mediator;

    public CofrinhoDbContext(DbContextOptions<CofrinhoDbContext> options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

    
    public DbSet<Objetivo> Objetivos { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Cofrinho");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        base.OnModelCreating(modelBuilder);
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
        }

        var result = await base.SaveChangesAsync(true, cancellationToken);
        
        await _mediator.DispatchDomainEvents(this);

        return result;
    }
}