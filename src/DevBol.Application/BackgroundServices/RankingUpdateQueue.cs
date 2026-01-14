using System.Threading.Channels;


namespace DevBol.Application.BackGroundsServices
{
    public class RankingUpdateQueue
    {
        // Criamos um canal ilimitado para mensagens de GUID (ID da Rodada)
        private readonly Channel<Guid> _queue;

        public RankingUpdateQueue()
        {
            _queue = Channel.CreateUnbounded<Guid>();
        }

        // Método para o Service escrever na fila
        public async Task EscreverJogoAsync(Guid rodadaId)
        {
            await _queue.Writer.WriteAsync(rodadaId);
        }

        // Método para o Worker ler da fila
        public async Task<Guid> LerJogoAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}