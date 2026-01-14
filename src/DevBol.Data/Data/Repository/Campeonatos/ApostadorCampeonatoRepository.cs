using DevBol.Domain.Models.Campeonatos;
using DevBol.Infrastructure.Data.Context;
using DevBol.Infrastructure.Data.Interfaces.Campeonatos;
using DevBol.Infrastructure.Data.Repository.Apostadores;
using DevBol.Infrastructure.Data.Repository.Apostas;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace DevBol.Infrastructure.Data.Repository.Campeonatos
{
    public class ApostadorCampeonatoRepository : Repository<ApostadorCampeonato>, IApostadorCampeonatoRepository
    {

        private readonly DbContext MeuDbContext;
        private readonly ILogger<ApostadorCampeonatoRepository> _logger;

        public ApostadorCampeonatoRepository(MeuDbContext context,
                                              ILogger<ApostadorCampeonatoRepository> logger) : base(context)
        {

            _logger = logger;
        }
        public async Task<ApostadorCampeonato> ObterApostadorCampeonato(Guid id)
        {

            return await Db.ApostadoresCampeonatos.AsNoTracking()
            .Include(c => c.Apostador.Usuario)
            .FirstOrDefaultAsync(c => c.Id == id);

        }
        public async Task<ApostadorCampeonato> ObterApostadorDoCampeonato(Guid campeonatoId, Guid apostadorId)
        {
            return await Db.ApostadoresCampeonatos.AsNoTracking()
                               .Where(ec => ec.CampeonatoId == campeonatoId && ec.ApostadorId == apostadorId)
                               .FirstOrDefaultAsync(ec => ec.CampeonatoId == campeonatoId && ec.ApostadorId == apostadorId);

        }

        public async Task<IEnumerable<ApostadorCampeonato>> ObterApostadoresDoCampeonato(Guid campeonatoId, bool trackChanges = false)
        {
            var query = Db.ApostadoresCampeonatos
                .Include(c => c.Campeonato)
                .Include(a => a.Apostador.Usuario)
                .Include(ac => ac.RankingRodadas)
                .Where(ec => ec.CampeonatoId == campeonatoId)
                .OrderBy(ec => ec.Apostador.Usuario.Apelido);

            return trackChanges ? await query.ToListAsync() : await query.AsNoTracking().ToListAsync();
        }


        public async Task<IEnumerable<ApostadorCampeonato>> ObterApostadoresEmOrdemDescrescenteDePontuacao(Guid campeonatoId, bool trackChanges = false)
        {
            var query = Db.ApostadoresCampeonatos
                .Where(ac => ac.CampeonatoId == campeonatoId)
                .OrderByDescending(ac => ac.Pontuacao);

            return trackChanges ? await query.ToListAsync() : await query.AsNoTracking().ToListAsync();
        }

    }

    }