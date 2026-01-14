using DevBol.Domain.Models.Apostas;
using DevBol.Infrastructure.Data.Base;

namespace DevBol.Infrastructure.Data.Interfaces.Apostas
{
    public interface IApostaRodadaRepository : IRepository<ApostaRodada>
    {

        Task<ApostaRodada> ObterApostaRodadaSalvaDoApostadorNaRodada(Guid rodadaId, Guid apostadorCampeonatoId);
        // Outros métodos específicos para ApostaRodada

        Task<ApostaRodada> ObterUltimaApostaRodadaDoApostadorNaRodada(Guid apostadorCampeonatoId, Guid rodadaId);

        Task<ApostaRodada> ObterStatusApostaRodada(Guid rodadaId, Guid apostadorCampeonatoId);

        Task<bool> ExisteApostaNaRodada(Guid rodadaId);

        Task<IEnumerable<ApostaRodada>> ObterApostasRodadaApostadorNaRodada(Guid rodadaId, Guid apostadorCampeonatoId);

        Task<List<Guid>> ObterApostadoresDaRodada(Guid rodadaId);

        Task<IEnumerable<Palpite>> ObterPalpitesDoJogo(Guid jogoId, bool asNoTracking = true);

    }

}