using DevBol.Domain.Models.Estadios;

namespace DevBol.Application.Interfaces.Estadios
{
    public interface IEstadioService //: IDisposable
    {
        Task Adicionar(Estadio estadio);
        Task Atualizar(Estadio estadio);
        Task Remover(Guid id);

        Task RemoverEntity(Estadio estadio);
    }
}
