using DevBol.Domain.Models.Jogos;
using DevBol.Infrastructure.Data.Base;

namespace DevBol.Infrastructure.Data.Interfaces.Jogos
{
    public interface IJogoRepository : IRepository<Jogo>
    {
        //Task<Jogo> ObterJogo(Guid id);
        Task<Jogo> ObterJogo(Guid id, bool track = true); // Adiciona o parâmetro 'track' com valor padrão true

        Task <IEnumerable<Jogo>> ObterJogosRodada();
        
        Task<Jogo> ObterJogoRodada(Guid rodadaId);

        Task<IEnumerable<Jogo>> ObterJogosDaRodada(Guid rodadaId);

        Task<Jogo> ObterJogoEstadioEquipes(Guid id);

        Task<IEnumerable<Jogo>> ObterJogosNaoFinalizadosNaRodada(Guid rodadaId);

        Task<bool> ExistemJogosNaoFinalizadosNaRodada(Guid rodadaId);

    }
}