using DevBol.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using DevBol.Infrastructure.Data.Interfaces.RankingRodadas;
using DevBol.Domain.Models.RankingRodadas;
using Microsoft.Extensions.Logging;
//using System.Data.Entity;

namespace DevBol.Infrastructure.Data.Repository.RankingRodadas
{
    public class RankingRodadaRepository : Repository<RankingRodada>, IRankingRodadaRepository
    {
        private readonly DbContext MeuDbContext;
        private readonly ILogger<RankingRodadaRepository> _logger;

        public RankingRodadaRepository(MeuDbContext context,
                                       ILogger<RankingRodadaRepository> logger) : base(context)
        {
            _logger = logger;           

        }

        public async Task<IEnumerable<RankingRodada>> ObterRankingRodada(Guid id)
        {
            return await Db.RankingRodadas.AsNoTracking()
                         .Where(r => r.RodadaId == id)
                         .ToListAsync();
              
                       
        }

        public async Task<IEnumerable<RankingRodada>> ObterRankingDaRodadaEmOrdemDePontuacao(Guid idRodada)
        {
           
                return await Db.RankingRodadas.AsNoTracking()                
                //.Include(r => r.Rodada)
                //.Include(r => r.Rodada.Campeonato)
                .Include(r => r.ApostadorCampeonato.Apostador.Usuario)
                .Where(r => r.RodadaId == idRodada)
                .OrderByDescending(r => r.Pontuacao) // Ordenar por pontuação
                .ToListAsync();
            
        }

        public async Task<RankingRodada> ObterRankingDoApostadorNaRodada(Guid idRodada, Guid idApostador)
        {
           
                return await Db.RankingRodadas.AsNoTracking()
                //return await Db.RankingRodadas
                //.Include(r => r.Rodada)
                //.Include(r => r.Rodada.Campeonato)
                .Include(r => r.ApostadorCampeonato.Apostador)
                //.Where(r => r.RodadaId == idRodada && r.ApostadorCampeonatoId == idApostador).ToListAsync();
                .FirstOrDefaultAsync(r => r.Rodada.Id == idRodada && r.ApostadorCampeonato.Id == idApostador);
           
        }

        /*public async Task Atualizar(RankingRodada rankingRodada, MeuDbContext context)
        {
            context.RankingRodadas.Update(rankingRodada);
            // NÃO chame context.SaveChangesAsync() aqui.
        }*/

    }

}