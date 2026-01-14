using DevBol.Domain.Models.Ufs;
using DevBol.Domain.Models.Equipes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace DevBol.Infrastructure.Data.Mappings
{
    public class UfMapping : IEntityTypeConfiguration<Uf>
    {
        public void Configure(EntityTypeBuilder<Uf> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nome)
                   .IsRequired()  
                   .HasColumnType("varchar(40)");

            builder.Property(e => e.Sigla)
                   .IsRequired()
                   .HasColumnType("varchar(2)");

            // 0 : N => Uf : Equipes - uma equipe pode não ter UF (EX: Seleções)

            builder.HasMany(u => u.Equipes)
              .WithOne(e => e.Uf)
              .HasForeignKey(e => e.UfId)
              .IsRequired(false);

            // 0 : N => Uf : Estadios - pode haver estádios sem UF (Copa do Mundo, etc)
            builder.HasMany(u => u.Estadios)
              .WithOne(e => e.Uf)
              .HasForeignKey(e => e.UfId)
              .IsRequired(false);

            builder.ToTable("Ufs");
        }
    }
}