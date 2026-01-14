using DevBol.Domain.Models.Campeonatos;
using DevBol.Domain.Models.Rodadas;

namespace DevBol.Application.Interfaces.Campeonatos
{
    public interface IEquipeCampeonatoService // : IDisposable
    {
        Task Adicionar(EquipeCampeonato equipeCampeonato);
        //Task Atualizar(EquipeCampeonato equipeCampeonato);
        Task RemoverEntity(EquipeCampeonato equipeCampeonato);

    }
}
