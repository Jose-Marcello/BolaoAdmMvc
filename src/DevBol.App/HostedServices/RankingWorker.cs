using DevBol.Application.BackGroundsServices;
using DevBol.Application.Interfaces.RankingRodadas;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NuGet.Protocol;

namespace DevBol.Presentation.BackgroundServices
{
    public class RankingWorker : BackgroundService
    {
        private readonly LancamentoPlacarQueue _queue; // 1. Usando a nova fila
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RankingWorker> _logger;

        public RankingWorker(
               LancamentoPlacarQueue queue,
               IServiceScopeFactory scopeFactory,
               ILogger<RankingWorker> logger)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(">>> [WORKER] O MOTOR DE PROCESSAMENTO DE PLACARES ACORDOU!");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // 2. Fica escutando a fila. Quando a Controller postar o JogoId, ele acorda.
                    var jogoId = await _queue.LerRodadaAsync(stoppingToken);

                    _logger.LogInformation($">>> [WORKER] INICIANDO PROCESSAMENTO DO JOGO: {jogoId}");

                    using (var scope = _scopeFactory.CreateScope())
                    {
                        // 3. Obtemos o Service que agora contém o LOOP PESADO e o RANKING
                        var rankingService = scope.ServiceProvider.GetRequiredService<IRankingRodadaService>();

                        // 4. CHAMA A BOMBA (Processa pontos dos 500+ apostadores e reordena o ranking)
                        await rankingService.ProcessarPontuacaoERankingAsync(jogoId);

                        _logger.LogInformation($">>> [WORKER] JOGO {jogoId} PROCESSADO COM SUCESSO!");
                    }
                }
                catch (Exception ex)              {
                    _logger.LogError($">>> [WORKER] ERRO AO PROCESSAR RANKING: {ex.Message}");
                }
            }
        }
    }
}