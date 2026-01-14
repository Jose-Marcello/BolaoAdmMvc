using DevBol.Infrastructure.Data.Context;
using DevBol.Domain.Models.Campeonatos;
using Microsoft.EntityFrameworkCore;
using DevBol.Infrastructure.Data.Interfaces.Campeonatos;
using DevBol.Infrastructure.Data.Repository.Apostas;
using Microsoft.Extensions.Logging;

namespace DevBol.Infrastructure.Data.Repository.Campeonatos
{
    public class CampeonatoRepository : Repository<Campeonato>, ICampeonatoRepository
    {
            private readonly DbContext MeuDbContext;
            private readonly ILogger<CampeonatoRepository> _logger;

            public CampeonatoRepository(MeuDbContext context,
                                          ILogger<CampeonatoRepository> logger) : base(context)
            {               
                _logger = logger;
            }

        public async Task<Campeonato> ObterCampeonato(Guid id)
        {
          

                return await Db.Campeonatos.AsNoTracking()
               .FirstOrDefaultAsync(c => c.Id == id);
            
        }
        
        public async Task<Campeonato> ObterCampeonatoAtivo()
        {
           
                //aqui serve para a lógica de que só há um CAMPEONATO ATIVO
                return await Db.Campeonatos.AsNoTracking()
                         .Where(c => c.Ativo == true).FirstAsync(c => c.Ativo == true);
                //.FirstOrDefaultAsync(c => c.Ativo == true);
                //.ToListAsync();
           

        }

        public async Task<IEnumerable<Campeonato>> ObterListaDeCampeonatosAtivos()
        {
          
                return await Db.Campeonatos.AsNoTracking()
                         .Where(c => c.Ativo == true)
                         .ToListAsync();
            

        }

        public async Task<IEnumerable<Campeonato>> ObterCampeonatosPorTipo()
        {
           
                return await Db.Campeonatos.AsNoTracking()
                .OrderBy(c => c.Tipo).ToListAsync();
            
        }

        //aqui tem que reformular !!!
        public async Task<Campeonato> ObterCampeonatoRodadas(Guid id)
        {
            

                return await Db.Campeonatos.AsNoTracking()
                 .Include(c => c.Rodadas)
                 .FirstOrDefaultAsync(c => c.Id == id);
           

        }

       
    }

}