using DevBol.Domain.Models.Ufs;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using DevBol.Domain.Models.Jogos;

public class JogoMapping : IEntityTypeConfiguration<Jogo>
{
    public void Configure(EntityTypeBuilder<Jogo> builder)
    {
        builder.HasKey(j => j.Id);
        
        builder.Property(j => j.DataJogo)
                .IsRequired();

        builder.Property(j => j.HoraJogo)
                .IsRequired();

        // 1 : N => Jogo : Palpites (ATUALIZADO!)
        builder.HasMany(j => j.Palpites) // <-- AGORA APONTA PARA A COLEÇÃO DE PALPITES
            .WithOne(p => p.Jogo)
            .HasForeignKey(p => p.JogoId)
            .IsRequired(true);

        // 1 : N
        builder.ToTable("Jogos");
    }
}

