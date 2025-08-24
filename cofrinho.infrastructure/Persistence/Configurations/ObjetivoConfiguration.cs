using cofrinho.core.Common.Extensions;
using cofrinho.core.Entities;
using cofrinho.core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cofrinho.infrastructure.Persistence.Configurations;

public class ObjetivoConfiguration : IEntityTypeConfiguration<Objetivo>
{
    public void Configure(EntityTypeBuilder<Objetivo> builder)
    {
        builder.Ignore(x => x.Notifications).Ignore(x => x.IsValid);
        builder.HasKey(x => x.Id);

        builder.OwnsOne(o => o.ValorAlvo, on =>
        {
            on.Ignore(x => x.Notifications);
            on.Ignore(x => x.IsValid);
            
            on.Property(x => x.Valor).HasColumnName("ValorAlvo").HasPrecision(18,2);
            on.Property(x => x.MoedaEnum).HasConversion<string>().HasColumnName("ValorAlvo_Moeda");
        });

        builder.OwnsOne(o => o.ImagemUrl, on =>
        {
            on.Ignore(x => x.Notifications);
            on.Ignore(x => x.IsValid);
            on.Property(x => x.Valor).HasColumnName("ImagemUrl");

        });

        builder.Property(o => o.Titulo).HasMaxLength(50).IsRequired();
        builder.Property(o => o.Descricao).HasMaxLength(255).IsRequired(false);
        builder.Property(o => o.Prazo).IsRequired(false);
        builder.Property(o => o.Status).HasConversion<string>().IsRequired();
        builder.Property(o => o.Categoria).HasConversion<string>().IsRequired();
        
        builder.HasMany<Transacao>()
            .WithOne(x => x.Objetivo)
            .HasForeignKey(x => x.ObjetivoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}