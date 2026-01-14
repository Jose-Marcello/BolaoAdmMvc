using DevBol.Domain.Models.Equipes;
using DevBol.Infrastructure.Data.Base;

namespace DevBol.Infrastructure.Data.Interfaces.Equipes
{
    public interface IEquipeRepository : IRepository<Equipe>
    {
        Task<Equipe> ObterEquipe(Guid id);
       
        Task <IEnumerable<Equipe>> ObterEquipesUf();
        
        //Task<Equipe> ObterEquipeRodada(Guid campeonatoId);

    }
}