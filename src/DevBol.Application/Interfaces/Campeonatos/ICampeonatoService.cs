using DevBol.Domain.Models.Campeonatos;

namespace DevBol.Application.Interfaces.Campeonatos
{
    public interface ICampeonatoService //: IDisposable
    {
        Task Adicionar(Campeonato campeonato);
        Task Atualizar(Campeonato campeonato);
        Task Remover(Guid id);
        Task RemoverEntity(Campeonato campeonato);

    }
}
