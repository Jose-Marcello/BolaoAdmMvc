using DevBol.Domain.Models.Ufs;

namespace DevBol.Application.Interfaces.Ufs
{
    public interface IUfService //: IDisposable
    {
        Task Adicionar(Uf uf);
        Task Atualizar(Uf uf);
        Task Remover(Guid id);

    }
}
