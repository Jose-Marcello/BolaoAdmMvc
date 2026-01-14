using DevBol.Domain.Models.Apostadores;
using DevBol.Domain.Models.Campeonatos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBol.Infrastructure.Data.Mappings
{
    public class ApostadorMapping : IEntityTypeConfiguration<JogoFinalizadoEvent>
    {
        public void Configure(EntityTypeBuilder<JogoFinalizadoEvent> builder)
        {
            builder.HasKey(c => c.Id);

            /* builder.Property(c => c.Nome)
                 .IsRequired()
                 .HasColumnType("varchar(60)");

             builder.Property(c => c.CPF)
                 .IsRequired()
                 .HasColumnType("varchar(11)");

             builder.Property(c => c.DataCadastro)
                 .IsRequired();

             builder.Property(c => c.Email)
                 .IsRequired();

             builder.Property(c => c.Celular)
                 .IsRequired();
                         */

            builder.ToTable("Apostadores");
        }
    }
}