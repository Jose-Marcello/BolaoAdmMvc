using DevBol.Domain.Models.Apostadores;
using DevBol.Domain.Models.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevBol.Infrastructure.Data.Mappings
{
    public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(c => c.Id);

            /* builder.Property(u => u.NomeCompleto)
                 .IsRequired()
                .HasColumnType("varchar(100)"); */

            builder.Property(u => u.Celular)
                .IsRequired()
               .HasColumnType("varchar(20)");

            builder.Property(u => u.CPF)
                .IsRequired()
               .HasColumnType("varchar(11)");

            builder.Property(u => u.Apelido)
                .IsRequired()
               .HasColumnType("varchar(50)");

            builder.HasOne(u => u.Apostador)
            .WithOne(a => a.Usuario)
            .HasForeignKey<JogoFinalizadoEvent>(a => a.UsuarioId);

            builder.ToTable("AspNetUsers");
        }
    }
}