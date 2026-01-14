using DevBol.Domain.Models.Apostadores;
namespace DevBol.Application.Interfaces.Apostadores

{
    public interface IApostadorService // : IDisposable
    {
        Task Adicionar(JogoFinalizadoEvent apostador);
        Task Atualizar(JogoFinalizadoEvent apostador);
        Task Remover(Guid id);
        Task RemoverEntity(JogoFinalizadoEvent apostador);

    }
}
