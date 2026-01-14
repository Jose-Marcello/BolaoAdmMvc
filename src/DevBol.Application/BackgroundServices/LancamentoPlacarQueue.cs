using System.Threading.Channels;

namespace DevBol.Application.BackGroundsServices
{
    /// <summary>
    /// Gerencia a fila de eventos de lançamento de placar.
    /// Esta classe é injetada como Singleton para ser o canal de rádio entre a Controller e o Worker.
    /// </summary>
    public class LancamentoPlacarQueue
    {
        // O Channel é thread-safe e otimizado para cenários de produtor-consumidor
        private readonly Channel<Guid> _queue;

        public LancamentoPlacarQueue()
        {
            // Criamos um canal ilimitado. O Guid transportado é o RodadaId.
            _queue = Channel.CreateUnbounded<Guid>();
        }

        // Método chamado pela Controller (Produtor)
        public async Task EscreverRodadaAsync(Guid rodadaId)
        {
            await _queue.Writer.WriteAsync(rodadaId);
        }

        // Método chamado pelo Worker (Consumidor)
        public async Task<Guid> LerRodadaAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}