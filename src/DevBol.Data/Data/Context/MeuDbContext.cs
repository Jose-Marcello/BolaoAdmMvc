using DevBol.Domain.Models.Apostadores;
using DevBol.Domain.Models.Apostas;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Domain.Models.Equipes;
using DevBol.Domain.Models.Estadios;
using DevBol.Domain.Models.Financeiro;
using DevBol.Domain.Models.Jogos;
using DevBol.Domain.Models.RankingRodadas;
using DevBol.Domain.Models.Rodadas;
using DevBol.Domain.Models.Ufs;
using DevBol.Domain.Models.Usuarios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace DevBol.Infrastructure.Data.Context
{

    public class MeuDbContext : IdentityDbContext<Usuario, IdentityRole, string>
    {
        //public MeuDbContext(DbContextOptions<MeuDbContext> options) : base(options)
        public MeuDbContext(DbContextOptions<MeuDbContext> options) : base(options)
        {
           
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<Campeonato> Campeonatos { get; set; }
        public DbSet<Rodada> Rodadas { get; set; }
        public DbSet<Equipe> Equipes { get; set; }
        public DbSet<Uf> Ufs { get; set; }
        public DbSet<EquipeCampeonato> EquipesCampeonatos { get; set; }
        public DbSet<Estadio> Estadios { get; set; }
        public DbSet<Jogo> Jogos { get; set; }
        public DbSet<JogoFinalizadoEvent> Apostadores { get; set; }
        public DbSet<ApostadorCampeonato> ApostadoresCampeonatos { get; set; }
        public DbSet<Palpite> Palpites { get; set; }
        public DbSet<RankingRodada> RankingRodadas { get; set; }        
        public DbSet<ApostaRodada> ApostasRodada { get; set; }
        public DbSet<TransacaoFinanceira> TransacoesFinanceiras { get; set; }
        public DbSet<Saldo> Saldos { get; set; }      

        public DbSet<Usuario> Usuarios { get; set; }        

        protected override void OnModelCreating(ModelBuilder modelBuilder)       {
           

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeuDbContext).Assembly);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataCadastro") != null))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("DataCadastro").CurrentValue = DateTime.Now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("DataCadastro").IsModified = false;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}