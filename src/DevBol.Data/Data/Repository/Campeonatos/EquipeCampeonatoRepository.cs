using DevBol.Infrastructure.Data.Context;
using DevBol.Domain.Models.Campeonatos;
using Microsoft.EntityFrameworkCore;
using DevBol.Domain.Models.Equipes;
using DevBol.Domain.Models.Rodadas;
using DevBol.Infrastructure.Data.Repository;
using DevBol.Infrastructure.Data.Interfaces.Campeonatos;
using Microsoft.Extensions.Logging;
using DevBol.Infrastructure.Data.Repository.Apostas;

namespace DevBol.Infrastructure.Data.Repository.Campeonatos
{
    public class EquipeCampeonatoRepository : Repository<EquipeCampeonato>, IEquipeCampeonatoRepository
    {

        private readonly DbContext MeuDbContext;
        private readonly ILogger<EquipeCampeonatoRepository> _logger;

        public EquipeCampeonatoRepository(MeuDbContext context,
                                            ILogger<EquipeCampeonatoRepository> logger) : base(context)
        {
           
            _logger = logger;
        }

        public async Task<EquipeCampeonato> ObterEquipeCampeonato(Guid campeonatoId, Guid equipeId)
        {
           

                return await Db.EquipesCampeonatos.AsNoTracking()
                               .Where(ec => ec.CampeonatoId == campeonatoId && ec.EquipeId == equipeId)
                               .FirstOrDefaultAsync(ec => ec.CampeonatoId == campeonatoId && ec.EquipeId == equipeId);
           
        }

        public async Task<IEnumerable<EquipeCampeonato>> ObterEquipesDoCampeonato(Guid campeonatoId)
        {
                            return await Db.EquipesCampeonatos.AsNoTracking()
                                   .Include(c => c.Campeonato)
                                   .Include(e => e.Equipe)
                                   .Include(u => u.Equipe.Uf)
                                   .Where(ec => ec.CampeonatoId == campeonatoId)
                                   .OrderBy(ec => ec.Equipe.Nome).ToListAsync();

           

    }

    }

}