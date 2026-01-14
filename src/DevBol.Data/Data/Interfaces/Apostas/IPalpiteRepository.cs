using DevBol.Domain.Models.Apostas;
using DevBol.Infrastructure.Data.Base;


namespace DevBol.Infrastructure.Data.Interfaces.Apostas
{
    public interface IPalpiteRepository : IRepository<Palpite>
    {
        // Obtém todos os palpites para uma rodada específica, incluindo o jogo e o apostador
        Task<IEnumerable<Palpite>> ObterPalpitesDaRodada(Guid rodadaId);

        Task<int> ObterTotaldePontosdoApostadorNaRodada(Guid rodadaId, Guid apostadorCampeonatoId);

        Task<IEnumerable<Palpite>> ObterPalpitesDoApostadorNaRodada(Guid rodadaId, Guid ApostadorCampeonatoId);

        // Novo método para remover todos os palpites associados a uma rodada
        Task<bool> RemoverTodosPalpitesDaRodada(Guid rodadaId);
       
        Task AdicionarRange(IEnumerable<Palpite> entities); // <<-- ADICIONE ESTA LINHA SE NÃO TIVER -->>
       
    }
}
