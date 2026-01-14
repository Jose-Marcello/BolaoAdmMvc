using DevBol.Domain.Models.Rodadas;
using DevBol.Infrastructure.Data.Base;

namespace DevBol.Infrastructure.Data.Interfaces.Rodadas
{
    public interface IRodadaRepository : IRepository<Rodada>
    {
        Task<Rodada> ObterRodada(Guid id);

        Task<Rodada> ObterRodadaCorrente();

        Task<IEnumerable<Rodada>> ObterListaComRodadaCorrente();
        
        Task<IEnumerable<Rodada>> ObterRodadasCampeonato();

        //Task<IEnumerable<Rodada>> ObterRodadasEmConstrucao();
        Task<Rodada> ObterRodadaCampeonato(Guid campeonatoId);

        Task<Rodada> ObterRodadaProntaNaoIniciada();

        Task<IEnumerable<Rodada>> ObterRodadasDoCampeonato(Guid campeonatoId);

        Task<Rodada> ObterUltimaRodadaFinalizadaDoCampeonato(Guid campeonatoId);

        Task<IEnumerable<Rodada>> ObterRodadasPorStatus(StatusRodada Status);

      




    }
}