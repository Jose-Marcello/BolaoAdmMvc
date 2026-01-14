using DevBol.Domain.Models.Estadios;
using DevBol.Domain.Models.Equipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace DevBol.Infrastructure.Data.Mappings
{
    public class EstadioMapping : IEntityTypeConfiguration<Estadio>
    {
        public void Configure(EntityTypeBuilder<Estadio> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nome)
                   .IsRequired()
                   .HasColumnType("varchar(60)");            

            builder.ToTable("Estadios");
        }
    }
}
