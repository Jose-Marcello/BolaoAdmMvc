using DevBol.Infrastructure.Data.Context;
using DevBol.Domain.Models.Jogos;
using Microsoft.EntityFrameworkCore;
using DevBol.Infrastructure.Data.Interfaces.Jogos;
using Microsoft.Extensions.Logging;


namespace DevBol.Infrastructure.Data.Repository.Jogos
{
    public class JogoRepository : Repository<Jogo>, IJogoRepository
    {
        private readonly DbContext MeuDbContext;
        private readonly ILogger<JogoRepository> _logger;

        // Exponha o DbContext através de uma propriedade pública


        public JogoRepository(MeuDbContext context,
                                ILogger<JogoRepository> logger) : base(context)
        {

            _logger = logger;

        }

        public MeuDbContext Context => Db;

        public async Task<Jogo> ObterJogo(Guid id, bool track = true)
        //public async Task<Jogo> ObterJogo(Guid id)
        {

            /* return await Db.Jogos.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);    */

            var query = Db.Jogos.Where(r => r.Id == id);

            if (track)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<Jogo>> ObterJogosRodada()
        {

            return await Db.Jogos.AsNoTracking()
                             .Include(j => j.Rodada)
                             .Include(j => j.Rodada.Campeonato)
                             .OrderBy(j => j.DataJogo).ThenBy(j => j.HoraJogo).ToListAsync();


        }

        public async Task<Jogo> ObterJogoRodada(Guid id)
        {

            return await Db.Jogos.AsNoTracking()
            .Include(j => j.Rodada)
            .Include(j => j.Rodada.Campeonato)
            .Include(j => j.EquipeCasa.Equipe)
            .Include(j => j.EquipeVisitante.Equipe)
            .Include(j => j.Estadio)
            .FirstOrDefaultAsync(j => j.Id == id);

        }

        public async Task<Jogo> ObterJogoEstadioEquipes(Guid id)
        {

            return await Db.Jogos.AsNoTracking()
            //.Include(j => j.Rodada)
            //.Include(j => j.Rodada.Campeonato)
            .Include(j => j.EquipeCasa.Equipe)
            .Include(j => j.EquipeVisitante.Equipe)
            .Include(j => j.Estadio)
            .FirstOrDefaultAsync(j => j.Id == id);

        }


        //Lista Todas as Jogos de um Rodada selecionada
        //aqui melhorar ordenando por DATA+HORA
        public async Task<IEnumerable<Jogo>> ObterJogosDaRodada(Guid rodadaId)
        {

            return await Db.Jogos.AsNoTracking()
                      .Include(j => j.Rodada)
                      .Include(j => j.Estadio)
                      .Include(j => j.EquipeCasa.Equipe)
                      .Include(j => j.EquipeVisitante.Equipe)
                      .Where(j => j.RodadaId == rodadaId)
                      .OrderBy(j => j.DataJogo).ThenBy(j => j.HoraJogo).ToListAsync();


        }

        public async Task<bool> ExistemJogosNaoFinalizadosNaRodada(Guid rodadaId)
        {
            
            var jogo = await Db.Jogos.AsNoTracking()                      
                      .Where(j => j.RodadaId == rodadaId && j.Status != StatusJogo.Finalizado)
                      .FirstOrDefaultAsync(j => j.RodadaId == rodadaId);

            if (jogo == null)
            {

                return false;
            }

            return true;                    

        }

        public async Task<IEnumerable<Jogo>> ObterJogosNaoFinalizadosNaRodada(Guid rodadaId)
        {

            return await Db.Jogos.AsNoTracking()
               .Where(j => j.RodadaId == rodadaId && j.Status != StatusJogo.Finalizado)
               .ToListAsync();

        }
    }
}

