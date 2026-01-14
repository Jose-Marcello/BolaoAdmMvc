using DevBol.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using DevBol.Domain.Models.Estadios;
using DevBol.Infrastructure.Data.Interfaces.Estadios;
using Microsoft.Extensions.Logging;

namespace DevBol.Infrastructure.Data.Repository.Estadios
{
    public class EstadioRepository : Repository<Estadio>, IEstadioRepository
    {

        private readonly DbContext MeuDbcontext;
        private readonly ILogger<EstadioRepository> _logger;

        public EstadioRepository(MeuDbContext context,
                                  ILogger<EstadioRepository> logger) : base(context)
        {
            
            _logger = logger;
        }

        public async Task<Estadio> ObterEstadio(Guid id)
        {
            return await Db.Estadios.AsNoTracking()
               .Include(u => u.Uf)
               .FirstOrDefaultAsync(e => e.Id == id);


        }

        public async Task<IEnumerable<Estadio>> ObterEstadiosUf()
        {
           

                return await Db.Estadios.AsNoTracking()
                 .Include(u => u.Uf)
                 .OrderBy(e => e.Nome).ToListAsync();
           
        }

        public async Task<IEnumerable<Estadio>> ObterEstadiosEmOrdemAlfabetica()
        {
              return await Db.Estadios.AsNoTracking()
                .Include(e => e.Uf)
                .OrderBy(e => e.Nome).ToListAsync();

        }

    }
}
        