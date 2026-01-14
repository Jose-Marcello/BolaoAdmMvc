using DevBol.Domain.Models.Apostadores;
using DevBol.Infrastructure.Data.Base;

namespace DevBol.Infrastructure.Data.Interfaces.Apostadores
{
    public interface IApostadorRepository : IRepository<JogoFinalizadoEvent>
    {
        Task<JogoFinalizadoEvent> ObterApostador(Guid id);

        Task<IEnumerable<JogoFinalizadoEvent>> ObterApostadoresEmOrdemAlfabetica();


        //Só pode haver um campeonato ativo (Validar isso)
        //Task<Apostador> ObterApostadorAtivo();



    }
}