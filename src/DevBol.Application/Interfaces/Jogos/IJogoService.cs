using DevBol.Domain.Models.Estadios;
using DevBol.Domain.Models.Jogos;

namespace DevBol.Application.Interfaces.Jogos
{
    public interface IJogoService // : IDisposable
    {
        Task Adicionar(Jogo jogo);
        Task Atualizar(Jogo jogo);
        Task Remover(Guid id);

    }
}
