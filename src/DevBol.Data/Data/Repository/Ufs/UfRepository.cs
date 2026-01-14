using DevBol.Domain.Interfaces.Ufs;
using DevBol.Domain.Models.Ufs;
using DevBol.Infrastructure.Data.Context;
using DevBol.Infrastructure.Data.Repository.RankingRodadas;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DevBol.Infrastructure.Data.Repository
{
    public class UfRepository : Repository<Uf>, IUfRepository
    {

        private readonly DbContext MeuDbContext;
        private readonly ILogger<UfRepository> _logger;

        public UfRepository(MeuDbContext context,
                            ILogger<UfRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<Uf> ObterUf(Guid id)
        {

            return await Db.Ufs.AsNoTracking()
           .FirstOrDefaultAsync(e => e.Id == id);

        }

        public async Task<IEnumerable<Uf>> ObterUfsEmOrdemAlfabetica()
        {


            return await Db.Ufs.AsNoTracking().
            OrderBy(u => u.Nome).ToListAsync();

        }


    }

}
