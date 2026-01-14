using DevBol.Domain.Models.RankingRodadas;
using DevBol.Infrastructure.Data.Base;
using DevBol.Infrastructure.Data.Context;

namespace DevBol.Infrastructure.Data.Interfaces.RankingRodadas
{
    public interface IRankingRodadaRepository : IRepository<RankingRodada>
    {
        Task<IEnumerable<RankingRodada>> ObterRankingRodada(Guid id);

        Task<IEnumerable<RankingRodada>> ObterRankingDaRodadaEmOrdemDePontuacao(Guid idRodada);

        Task<RankingRodada> ObterRankingDoApostadorNaRodada(Guid idRodada, Guid idApostador);

        //colocado aqui, devido à necessidade de usar um contexto separado
        //Task Atualizar(RankingRodada rankingRodada, MeuDbContext context);


    }
}