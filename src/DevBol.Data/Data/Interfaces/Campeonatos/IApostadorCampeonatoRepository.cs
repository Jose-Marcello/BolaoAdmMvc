using DevBol.Domain.Models.Campeonatos;
using DevBol.Infrastructure.Data.Base;

namespace DevBol.Infrastructure.Data.Interfaces.Campeonatos
{
    public interface IApostadorCampeonatoRepository : IRepository<ApostadorCampeonato>
    {
        Task<ApostadorCampeonato> ObterApostadorCampeonato(Guid id);
        Task<ApostadorCampeonato> ObterApostadorDoCampeonato(Guid idCampeonato, Guid idApostador);
        Task<IEnumerable<ApostadorCampeonato>> ObterApostadoresDoCampeonato(Guid id, bool trackChanges);

        Task<IEnumerable<ApostadorCampeonato>> ObterApostadoresEmOrdemDescrescenteDePontuacao(Guid campeonatoId, bool trackChanges);



    }
}