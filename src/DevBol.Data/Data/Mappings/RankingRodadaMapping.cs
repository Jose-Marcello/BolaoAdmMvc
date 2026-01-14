using DevBol.Domain.Models.RankingRodadas;
using DevBol.Domain.Models.Rodadas;
using DevBol.Domain.Models.Campeonatos; // Adicione este using
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBol.Infrastructure.Data.Mappings
{
    public class RankingRodadaMapping : IEntityTypeConfiguration<RankingRodada>
    {
        public void Configure(EntityTypeBuilder<RankingRodada> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Pontuacao)
                .IsRequired();

            builder.Property(r => r.Posicao)
                .IsRequired();

            builder.Property(r => r.DataAtualizacao)
                .IsRequired();
                      

           /* // 1 : 1 => RankingRodada : ApostadorCampeonato
            builder.HasOne(r => r.ApostadorCampeonato)
                .WithOne()
                .HasForeignKey<RankingRodada>(r => r.ApostadorCampeonatoId)
                .IsRequired(true);

            builder.ToTable("RankingRodadas");*/
        }
    }
}