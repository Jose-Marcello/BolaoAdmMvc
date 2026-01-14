using DevBol.Domain.Models.RankingRodadas;


namespace DevBol.Application.Interfaces.RankingRodadas
{
    public interface IRankingRodadaService //: IDisposable
    {

        Task ProcessarPontuacaoERankingAsync(Guid jogoId);
        Task Adicionar(RankingRodada rankingRodada);
        Task Atualizar(RankingRodada rankingRodada);
        Task Remover(Guid id);

    }
}
