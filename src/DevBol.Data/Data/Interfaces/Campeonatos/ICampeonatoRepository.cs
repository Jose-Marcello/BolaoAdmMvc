using DevBol.Domain.Models.Campeonatos;
using DevBol.Infrastructure.Data.Base;
using System.Collections;

namespace DevBol.Infrastructure.Data.Interfaces.Campeonatos
{
    public interface ICampeonatoRepository : IRepository<Campeonato>
    {
        Task<Campeonato> ObterCampeonato(Guid id);
        Task <IEnumerable<Campeonato>> ObterCampeonatosPorTipo();
        
        //Só pode haver um campeonato ativo (Validar isso)
        Task<Campeonato> ObterCampeonatoAtivo();
        Task<IEnumerable<Campeonato>> ObterListaDeCampeonatosAtivos(); //A princípio, uma lista com apenas 1 (p/grid)

        Task<Campeonato> ObterCampeonatoRodadas(Guid Id);

    }
}