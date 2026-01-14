using Microsoft.EntityFrameworkCore;
using DevBol.Infrastructure.Data.Interfaces.Apostadores;
using DevBol.Domain.Models.Apostadores;
using DevBol.Infrastructure.Data.Context;
using Microsoft.Extensions.Logging;
using DevBol.Domain.Models.Ufs;

namespace DevBol.Infrastructure.Data.Repository.Apostadores
{
    public class ApostadorRepository : Repository<JogoFinalizadoEvent>, IApostadorRepository
    {

        private readonly DbContext MeuDbContext;
        private readonly ILogger<ApostadorRepository> _logger;

        public ApostadorRepository(MeuDbContext context,
                                    ILogger<ApostadorRepository> logger) : base(context)
        {          
            _logger = logger;
        }

        public async Task<JogoFinalizadoEvent> ObterApostador(Guid id)
        {        
                return await Db.Apostadores.AsNoTracking()
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(c => c.Id == id);
            
        }

        public async Task<IEnumerable<JogoFinalizadoEvent>> ObterApostadoresEmOrdemAlfabetica()
        {
            return await Db.Apostadores.AsNoTracking()
            .Include(a => a.Usuario)
            .OrderBy(a => a.Usuario.Apelido).ToListAsync();

        }
        //public async Task<IEnumerable<Apostador>> ObterApostadorAtivo()
        /* public async Task<Apostador> ObterApostadorAtivo()

         {
             //throw new NotImplementedException();

             return await Db.Apostadores.AsNoTracking()
                          //.Where(c => c.Ativo == true)
                          .FirstOrDefaultAsync(c => c.Ativo == true);
                          //.ToListAsync();

         }*/



    }

}