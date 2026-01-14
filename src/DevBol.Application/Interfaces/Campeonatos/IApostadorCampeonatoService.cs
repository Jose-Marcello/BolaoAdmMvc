using DevBol.Domain.Models.Campeonatos;
using DevBol.Domain.Models.Rodadas;

namespace DevBol.Application.Interfaces.Campeonatos
{
    public interface IApostadorCampeonatoService //: IDisposable
    {
        Task Adicionar(ApostadorCampeonato apostadorCampeonato);
        
        //task RemoverEntity(ApostadorCampeonato apostadorCampeonato);
        Task Remover(Guid Id);

    }
}
