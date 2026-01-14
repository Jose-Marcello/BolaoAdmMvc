using DevBol.Domain.Models.Rodadas;

namespace DevBol.Application.Interfaces.Rodadas
{
    public interface IRodadaService //: IDisposable
    {
        Task Adicionar(Rodada rodada);
        Task Atualizar(Rodada rodada);
        Task Remover(Guid id);

    }
}
