using DevBol.Domain.Models.Campeonatos;
using DevBol.Domain.Models.Rodadas;
using DevBol.Infrastructure.Data.Base;

namespace DevBol.Infrastructure.Data.Interfaces.Campeonatos
{
    public interface IEquipeCampeonatoRepository : IRepository<EquipeCampeonato>
    {
        Task<EquipeCampeonato> ObterEquipeCampeonato(Guid idCampeonato, Guid idEquipe);
        Task<IEnumerable<EquipeCampeonato>> ObterEquipesDoCampeonato(Guid id);

    }
}