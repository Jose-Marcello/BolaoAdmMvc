using DevBol.Domain.Models.Ufs;
using DevBol.Infrastructure.Data.Base;

namespace DevBol.Domain.Interfaces.Ufs
{
    public interface IUfRepository : IRepository<Uf>
    {
        Task<Uf> ObterUf(Guid id);

        Task<IEnumerable<Uf>> ObterUfsEmOrdemAlfabetica();

        //Task<Equipe> ObterEquipeRodada(Guid campeonatoId);

    }
}