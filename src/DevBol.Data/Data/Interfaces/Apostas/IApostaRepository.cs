/*
using DevBol.Domain.Models.Apostas;
using DevBol.Domain.Models.Rodadas;
using DevBol.Infrastructure.Data.Base;

namespace DevBol.Infrastructure.Data.Interfaces.Apostas
{
    public interface IApostaRepository : IRepository<Aposta>
    {
        Task<Aposta> ObterAposta(Guid id);

        //Task<IEnumerable<Aposta>> ObterApostasJogoApostador();
        //Task<IEnumerable<Aposta>> ObterApostasJogoApostadorNaRodada(Guid rodadaId);


        Task<IEnumerable<Aposta>> ObterApostasDoJogo(Guid jogoId, bool asNoTracking = true);

        Task<IEnumerable<Aposta>> ObterApostasDaRodada(Guid rodadaId);
               
        Task<List<Guid>> ObterApostadoresDaRodada(Guid rodadaId);

        Task<IEnumerable<Aposta>> ObterApostasDoApostador(Guid apostadorId);

        Task<IEnumerable<Aposta>> ObterApostasDoApostadorNaRodada(Guid rodadaId, Guid apostadorId);

        Task<Aposta> ObterApostaSalvaDoApostadorNaRodada(Guid rodadaId, Guid apostadorId);
        Task<IEnumerable<Aposta>> ObterApostasSalvasNaRodada(Guid rodadaId);

        Task<bool> ExisteApostaNaRodada(Guid rodadaId);
        Task<bool> ExisteApostaSalvaNaRodada(Guid rodadaId);

    }
}
*/