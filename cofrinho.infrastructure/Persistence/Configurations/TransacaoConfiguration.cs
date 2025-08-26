using cofrinho.core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cofrinho.infrastructure.Persistence.Configurations;

public class TransacaoConfiguration : IEntityTypeConfiguration<Transacao>
{
    public void Configure(EntityTypeBuilder<Transacao> builder)
    {
        builder.Ignore(x => x.Notifications).Ignore(x => x.IsValid);
        builder.HasKey(x => x.Id);
        
        builder.OwnsOne(o => o.Valor, on =>
        {
            on.Ignore(x => x.Notifications);
            on.Ignore(x => x.IsValid);
            
            
            on.Property(x => x.Valor).HasColumnName("Valor").HasPrecision(18,2).IsRequired();
            on.Property(x => x.MoedaEnum).HasConversion<string>().HasColumnName("Valor_Moeda").IsRequired();
        });
        
        builder.Property(o => o.Tipo).HasConversion<string>().IsRequired();
        builder.Property(o => o.DataTransacao).IsRequired();
        
    }
    
}