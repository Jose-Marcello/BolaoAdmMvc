using DevBol.Infrastructure.Data.Context;
using DevBol.Domain.Models.Equipes;
using Microsoft.EntityFrameworkCore;
using DevBol.Domain.Models.Rodadas;
using DevBol.Infrastructure.Data.Repository;
using DevBol.Infrastructure.Data.Interfaces.Equipes;
using DevBol.Infrastructure.Data.Repository.Apostas;
using Microsoft.Extensions.Logging;

namespace DevBol.Infrastructure.Data.Repository.Equipes
{
    public class EquipeRepository : Repository<Equipe>, IEquipeRepository
    {
        private readonly DbContext MeuDbContext;
        private readonly ILogger<EquipeRepository> _logger;

        public EquipeRepository(MeuDbContext context,
                                ILogger<EquipeRepository> logger) : base(context)        {
       
            _logger = logger;
        }
        public async Task<Equipe> ObterEquipe(Guid id)
        {
           
                return await Db.Equipes.AsNoTracking()
               .Include(u => u.Uf)
               .FirstOrDefaultAsync(e => e.Id == id);
           

        }


        public async Task<IEnumerable<Equipe>> ObterEquipesUf()
        {
         
                return await Db.Equipes.AsNoTracking()
                 .Include(u => u.Uf)
                 .OrderBy(e => e.Nome).ToListAsync();
            
        }

        
    }

}