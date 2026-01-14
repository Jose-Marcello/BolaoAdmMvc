using DevBol.Domain.Models.Equipes;

namespace DevBol.Application.Interfaces.Equipes
{
    public interface IEquipeService //: IDisposable
    {
        Task Adicionar(Equipe equipe);
        Task Atualizar(Equipe equipe);
        Task Remover(Guid id);
        
        Task RemoverEntity(Equipe equipe);
    }
}
