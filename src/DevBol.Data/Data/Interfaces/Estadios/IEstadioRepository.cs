using DevBol.Domain.Models.Estadios;
using DevBol.Infrastructure.Data.Base;

namespace DevBol.Infrastructure.Data.Interfaces.Estadios
{
    public interface IEstadioRepository : IRepository<Estadio>
    {
        Task<Estadio> ObterEstadio(Guid id);

        Task<IEnumerable<Estadio>> ObterEstadiosUf();
        Task<IEnumerable<Estadio>> ObterEstadiosEmOrdemAlfabetica();



    }
}