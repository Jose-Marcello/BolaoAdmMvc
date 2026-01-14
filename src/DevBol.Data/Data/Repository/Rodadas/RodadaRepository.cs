using DevBol.Infrastructure.Data.Context;
using DevBol.Domain.Models.Rodadas;
using Microsoft.EntityFrameworkCore;
using DevBol.Infrastructure.Data.Interfaces.Rodadas;
using Microsoft.Extensions.Logging;
using DevBol.Domain.Models.Campeonatos;

namespace DevBol.Infrastructure.Data.Repository.Rodadas
{
    public class RodadaRepository : Repository<Rodada>, IRodadaRepository
    {
        private readonly DbContext MeuDbContext;
        private readonly ILogger<RodadaRepository> _logger;

        public RodadaRepository(MeuDbContext context,
                                 ILogger<RodadaRepository> logger) : base(context)
        {

            _logger = logger;
        }

        public async Task<Rodada> ObterRodada(Guid id)
        {

            return await Db.Rodadas.AsNoTracking()
            .Include(c => c.Campeonato)
           .FirstOrDefaultAsync(r => r.Id == id);

        }

        public async Task<Rodada> ObterRodadaCorrente()

        {

            return await Db.Rodadas.AsNoTracking()
                 .Include(r => r.Campeonato)
                 //.Where(r => r.Ativa)
                 .Where(r => r.Status == StatusRodada.Corrente)
                 .FirstOrDefaultAsync(r => r.Status == StatusRodada.Corrente);
            //.ToListAsync();


        }


        public async Task<IEnumerable<Rodada>> ObterRodadasPorStatus(StatusRodada status)
        {
            return await Db.Rodadas.AsNoTracking()
                .Include(r => r.Campeonato)
                .Where(r => r.Status == status) 
                .ToListAsync();
        }      

        public async Task<Rodada> ObterRodadaProntaNaoIniciada()

        {


            return await Db.Rodadas.AsNoTracking()
                     .Include(r => r.Campeonato)
                     //.Where(r => r.Ativa)
                     //.Where(r => r.Status == StatusRodada.NaoIniciada)
                     .FirstOrDefaultAsync(r => r.Status == StatusRodada.NaoIniciada);
            //.ToListAsync();


        }

        public async Task<IEnumerable<Rodada>> ObterListaComRodadaCorrente()

        {

            return await Db.Rodadas.AsNoTracking()
                     .Include(r => r.Campeonato)
                     //.Where(r => r.Ativa == true)
                     .Where(r => r.Status == StatusRodada.Corrente)
                     //.FirstOrDefaultAsync(r => r.Ativa == true);
                     .ToListAsync();


        }

        public async Task<IEnumerable<Rodada>> ObterRodadasCampeonato()
        {

            return await Db.Rodadas.AsNoTracking().Include(c => c.Campeonato)
                               .OrderBy(r => r.NumeroRodada).ToListAsync();


        }

        public async Task<Rodada> ObterRodadaCampeonato(Guid id)
        {

            return await Db.Rodadas.AsNoTracking().Include(c => c.Campeonato)
            .FirstOrDefaultAsync(r => r.Id == id);

        }


        //Lista Todas as Rodadas de um Campeonato selecionado
        public async Task<IEnumerable<Rodada>> ObterRodadasDoCampeonato(Guid campeonatoId)
        {

            return await Db.Rodadas.AsNoTracking().Include(c => c.Campeonato)
                               .Where(r => r.CampeonatoId == campeonatoId)
                               .OrderBy(r => r.NumeroRodada).ToListAsync();
        }

        public async Task<Rodada> ObterUltimaRodadaFinalizadaDoCampeonato(Guid campeonatoId)
        {
            return await Db.Rodadas.AsNoTracking()
                        .Include(r => r.Campeonato)
                        .Where(r => r.CampeonatoId == campeonatoId
                               && r.Status == StatusRodada.Finalizada)
                        .OrderByDescending(r => r.DataFim)
                        .FirstOrDefaultAsync();


        }
    }

}