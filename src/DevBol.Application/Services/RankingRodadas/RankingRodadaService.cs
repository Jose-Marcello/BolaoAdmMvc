using DevBol.Application.Interfaces.RankingRodadas;
using DevBol.Application.Base;
using DevBol.Domain.Models.RankingRodadas;
using DevBol.Domain.Interfaces;
using DevBol.Infrastructure.Data.Interfaces.RankingRodadas;
using DevBol.Infrastructure.Data.Interfaces.Jogos;
using DevBol.Infrastructure.Data.Interfaces.Apostas;
using DevBol.Domain.Models.Jogos;
using DevBol.Domain.Models.Apostas;

namespace DevBol.Application.Services.Rodadas
{
    public class RankingRodadaService : BaseService, IRankingRodadaService
    {
        private readonly IRankingRodadaRepository _rankingRodadaRepository;
        private readonly IJogoRepository _jogoRepository;
        private readonly IApostaRodadaRepository _apostaRodadaRepository;
        private readonly IUnitOfWork _uow;

        public RankingRodadaService(IRankingRodadaRepository rankingRodadaRepository,
                                    IJogoRepository jogoRepository,
                                    IApostaRodadaRepository apostaRodadaRepository,
                                    INotificador notificador,
                                    IUnitOfWork uow) : base(notificador, uow )
        {
            _rankingRodadaRepository = rankingRodadaRepository;
            _jogoRepository = jogoRepository;
            _apostaRodadaRepository = apostaRodadaRepository;
            _uow = uow;

        }

        // <<-- O NOVO MÉTODO PROCESSADOR (A BOMBA REORGANIZADA) -->>
        public async Task ProcessarPontuacaoERankingAsync(Guid jogoId)
        {
            // 1. Busca os dados do jogo e os palpites associados
            var jogo = await _jogoRepository.ObterJogo(jogoId);
            if (jogo == null) return;

            var palpitesParaAtualizar = await _apostaRodadaRepository.ObterPalpitesDoJogo(jogoId, asNoTracking: false);

            var placarCasaReal = jogo.PlacarCasa ?? 0;
            var placarVisitaReal = jogo.PlacarVisita ?? 0;

            // Lógica para bônus de palpite único exato
            var palpitesExatos = palpitesParaAtualizar.Count(p => p.ApostaRodada.Enviada &&
                                         p.PlacarApostaCasa == placarCasaReal &&
                                         p.PlacarApostaVisita == placarVisitaReal);

            // 2. O LOOP PESADO (Agora rodando em background pelo Worker)
            foreach (var palpite in palpitesParaAtualizar)
            {
                if (palpite.ApostaRodada.Enviada)
                {
                    var pontuacao = CalcularPontuacao(placarCasaReal, placarVisitaReal,
                                                      palpite.PlacarApostaCasa, palpite.PlacarApostaVisita);

                    // Bônus se for o único a acertar o placar em cheio
                    if (pontuacao == 7 && palpitesExatos == 1) pontuacao += 5;

                    palpite.Pontos = pontuacao;
                    // Não chamamos Service aqui, o UoW rastreia as mudanças no objeto palpite
                }
            }

            // 3. Salva os pontos calculados de todos os apostadores
            await _uow.SaveChanges();

            // 4. CHAMA A REORDENAÇÃO DO RANKING (Lógica que já existia na Controller)
            await RecalcularPosicoesRanking(jogo.RodadaId);
        }

        private async Task RecalcularPosicoesRanking(Guid rodadaId)
        {
            // Aqui entra aquela lógica de:
            // 1. Buscar todos os RankingRodadas daquela rodada
            // 2. Somar os pontos totais
            // 3. Ordenar por Pontuação DESC
            // 4. Atualizar a propriedade .Posicao (1º, 2º, 3º...)

            var rankingOrdenado = await _rankingRodadaRepository.ObterRankingDaRodadaEmOrdemDePontuacao(rodadaId);
            var lista = rankingOrdenado.ToList();

            int posicao = 1;
            int? pontosAnteriores = null;

            for (int i = 0; i < lista.Count; i++)
            {
                if (pontosAnteriores != null && lista[i].Pontuacao < pontosAnteriores)
                {
                    posicao = i + 1;
                }

                lista[i].Posicao = posicao;
                lista[i].DataAtualizacao = DateTime.Now;
                pontosAnteriores = (int)lista[i].Pontuacao;

                await _rankingRodadaRepository.Atualizar(lista[i]);
            }

            await _uow.SaveChanges();
        }

        private int CalcularPontuacao(int prc, int prv, int? pac, int? pav)
        {
            // Cole aqui aquela sua lógica de 7 pontos, 4 pontos, 3 pontos, etc.
            if (prc == pac && prv == pav) return 7;
            if (prc == prv && pac == pav) return 4;
            if ((prc > prv && pac > pav) || (prc < prv && pac < pav))
            {
                return (prc == pac || prv == pav) ? 4 : 3;
            }
            return 0;
        }


        // Métodos CRUD básicos mantidos abaixo...
        public async Task Adicionar(RankingRodada rankingRodada) { /* ... */ }
        public async Task Atualizar(RankingRodada rankingRodada) { /* ... */ }
        public async Task Remover(Guid id) { /* ... */ }


        /*
        [HttpPost]
        public async Task<IActionResult> SalvarJogo([Bind("Id,EquipeCasaId,EquipeVisitanteId,RodadaId,EstadioId,DataJogo,HoraJogo,PlacarCasa,PlacarVisita,Status")] JogoViewModel jogoViewModel)
        {
            try
            {
                // Verifica o ModelState antes de tentar recuperar dados
                if (!ModelState.IsValid)
                {
                    return View("EditarJogo", jogoViewModel);
                }

                // Obtendo e atualizando a entidade rastreada
                var jogoRepository = _uow.GetRepository<Jogo>() as JogoRepository;
                var jogoCorrente = await jogoRepository.ObterJogo(jogoViewModel.Id);

                // Verifique se o jogo foi encontrado antes de tentar atualizar
                if (jogoCorrente == null)
                {
                    ModelState.AddModelError("", "Jogo não encontrado.");
                    return View("EditarJogo", jogoViewModel);
                }

                // Atualiza as propriedades do objeto rastreável
                jogoCorrente.PlacarCasa = jogoViewModel.PlacarCasa;
                jogoCorrente.PlacarVisita = jogoViewModel.PlacarVisita;
                jogoCorrente.Status = jogoViewModel.Status;

                //atualização do jogo - Placar e status
                await _jogoService.Atualizar(jogoCorrente);

                // Lógica de validação de negócio com base no objeto de domínio
                if (jogoCorrente.Status == StatusJogo.NaoIniciado && (jogoCorrente.PlacarCasa != 0 || jogoCorrente.PlacarVisita != 0))
                {
                    ModelState.AddModelError("Placar", "O placar deve ser nulo para jogos não iniciados.");
                    return View("EditarJogo", jogoViewModel);
                }

                if ((jogoCorrente.Status == StatusJogo.EmAndamento ||
                     jogoCorrente.Status == StatusJogo.Finalizado) &&
                    (jogoCorrente.PlacarCasa == null || jogoCorrente.PlacarVisita == null))
                {
                    ModelState.AddModelError("Placar", "O placar não pode ser nulo para jogos em andamento ou finalizados.");
                    return View("EditarJogo", jogoViewModel);
                }

                // Se o ModelState ficou inválido após as validações de regra de negócio
                if (!ModelState.IsValid)
                {
                    return View("EditarJogo", jogoViewModel);
                }

                // Recalcula a pontuação dos usuários
                var apostaRodadaRepository = _uow.GetRepository<ApostaRodada>() as ApostaRodadaRepository;
                var palpitesParaAtualizar = await apostaRodadaRepository.ObterPalpitesDoJogo(jogoViewModel.Id, asNoTracking: false);

                var placarCasaReal = jogoCorrente.PlacarCasa.Value;
                var placarVisitaReal = jogoCorrente.PlacarVisita.Value;

                var palpitesExatos = palpitesParaAtualizar.Where(p => p.ApostaRodada.Enviada &&
                                                 p.PlacarApostaCasa == placarCasaReal &&
                                                 p.PlacarApostaVisita == placarVisitaReal).ToList();

                var numeroDePalpitesExatos = palpitesExatos.Count;
                

                // <<-- NOVA LÓGICA DE ATUALIZAÇÃO -->>
                // A lógica agora agrupa por ApostaRodada e depois recalcula o total
                var apostasComPalpitesDoJogo = palpitesParaAtualizar.Select(p => p.ApostaRodada).Distinct().ToList();

                //var PontuacaoAtual = apostasComPalpitesDoJogo.FirstOrDefault().PontuacaoTotalRodada;

                foreach (var apostaRodada in apostasComPalpitesDoJogo)
                {
                    // Obtém todos os palpites da aposta de rodada específica
                    var palpitesDaAposta = palpitesParaAtualizar.Where(p => p.ApostaRodadaId == apostaRodada.Id).ToList();
                    int pontuacaoTotalDaAposta = 0;

                    //apostaRodada.PontuacaoTotalRodada = 

                    foreach (var palpite in palpitesDaAposta)
                    {
                        if (palpite.ApostaRodada.Enviada)
                        {
                            var pontuacao = CalcularPontuacao(placarCasaReal, placarVisitaReal,
                                                             palpite.PlacarApostaCasa, palpite.PlacarApostaVisita);

                            if (pontuacao == 7 && numeroDePalpitesExatos == 1)
                            {
                                pontuacao += 5;
                            }

                            palpite.Pontos = pontuacao;
                            await _palpiteService.AtualizarPalpite(palpite);

                            pontuacaoTotalDaAposta += pontuacao; // Acumula a pontuação

                        }
                        // O palpite já está sendo rastreado pelo contexto. O SaveChanges no final irá atualizá-lo.
                    }
                    // Atualiza a pontuação total da ApostaRodada
                    //apostaRodada.PontuacaoTotalRodada = pontuacaoTotalDaAposta + apostaRodada.PontuacaoTotalRodada;
                    //await apostaRodadaRepository.Atualizar(apostaRodada);
                }

                // Finalmente, salva TODAS as alterações na transação da UoW
                await _uow.SaveChanges();

                // 2. DISPARE PARA A FILA (Substituindo a chamada direta)
                // O id que passamos é o jogoCorrente.RodadaId para o Worker saber qual ranking recalcular
                await _placarQueue.EscreverRodadaAsync(jogoCorrente.RodadaId);

                // Atualiza o ranking da rodada (em contexto isolado)
                //_ = AtualizarRankingRodada(jogoCorrente.RodadaId);

                return RedirectToAction("LancarPlacaresDaRodada");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao salvar jogo: {ex.Message}");
                return View("EditarJogo", jogoViewModel);
            }
        }
       
        

        //Método a ser excutado ao INICIAR JOGO e a cada alteração de PLACAR (a princípio, no PAINEL ADMINISTRATIVO)
        public int CalcularPontuacao(int placarRealCasa, int placarRealVisitante,
                                     int? palpiteCasa, int? palpiteVisitante)
        {
            int pontuacao = 0;

            // 1 - PLACAR EXATO: 7 PONTOS
            if (placarRealCasa == palpiteCasa && placarRealVisitante == palpiteVisitante)
            {
                pontuacao = 7;
            }
            // 2 - RESULTADO CORRETO, SENDO EMPATE, MAS SEM O PLACAR EXATO: 4 PONTOS
            else if (placarRealCasa == placarRealVisitante && palpiteCasa == palpiteVisitante)
            {
                pontuacao = 4;
            }
            // 3 - RESULTADO CORRETO, ACERTANDO O PLACAR DO VENCEDOR OU DO PERDEDOR: 4 PONTOS
            else if ((placarRealCasa > placarRealVisitante && palpiteCasa > palpiteVisitante) ||
                     (placarRealCasa < placarRealVisitante && palpiteCasa < palpiteVisitante))
            {
                if (placarRealCasa == palpiteCasa || placarRealVisitante == palpiteVisitante)
                {
                    pontuacao = 4;
                }
                // 4 - RESULTADO CORRETO, MAS SEM ACERTO DE PLACAR: 3 PONTOS
                else
                {
                    pontuacao = 3;
                }
            }
            // 5 - RESULTADO ERRADO, PORÉM ACERTANDO UM DOS PLACARES: 1 PONTO
            else if (placarRealCasa == palpiteCasa || placarRealVisitante == palpiteVisitante)
            {
                // Se a intenção era 1 ponto aqui, o código está incorreto.
                // No código original, pontuacao = 0; aqui.
                // Deixando como 0, conforme o original.
                pontuacao = 0;
            }
            // 6 - RESULTADO ERRADO, ERRANDO OS DOIS PLACARES: 0 PONTOS
            else
            {
                pontuacao = 0;
            }

            // 7 - BÔNUS PARA ACERTO DE PLACAR EXATO SOZINHO: 5 PONTOS
            // Este bônus já é tratado no loop do SalvarJogo, então essa parte não é mais necessária aqui
            // dentro de CalcularPontuacao. O cálculo base é feito aqui, e o bônus é adicionado depois.
            // if (pontuacao == 7)
            // {
            //     pontuacao += 5;
            // }
            return pontuacao;
        }

       
        [Route("AtualizarRankingRodada/{rodadaid:guid}")]
        public async Task<IActionResult> AtualizarRankingRodada(Guid rodadaId)
        {
            // --- Bloco para atualizar a PONTUAÇÃO ---
            using (var scopePontuacao = _serviceScopeFactory.CreateScope())
            {
                var uowPontuacao = scopePontuacao.ServiceProvider.GetRequiredService<IUnitOfWork>();
                // <<-- CORREÇÃO AQUI: Obtendo o repositório do novo escopo -->>
                var apostaRodadaRepository = scopePontuacao.ServiceProvider.GetRequiredService<IApostaRodadaRepository>() as ApostaRodadaRepository;
                var apostaRodadaService = scopePontuacao.ServiceProvider.GetRequiredService<IApostaRodadaService>() as ApostaRodadaService;
                var palpiteRepository = scopePontuacao.ServiceProvider.GetRequiredService<IPalpiteRepository>() as  PalpiteRepository;
                var rankingRodadaRepositoryPontuacao = scopePontuacao.ServiceProvider.GetRequiredService<IRankingRodadaRepository>() as RankingRodadaRepository;
                var rankingRodadaServicePontuacao = scopePontuacao.ServiceProvider.GetRequiredService<IRankingRodadaService>();

                try
                {
                    // <<-- CORREÇÃO AQUI: Usando o repositório do novo escopo -->>
                    var apostadores = await apostaRodadaRepository.ObterApostadoresDaRodada(rodadaId);
                    _logger.LogInformation($"Iniciando atualização da pontuação do ranking da rodada {rodadaId}");

                    foreach (var apostadorId in apostadores)
                    {
                        try
                        {
                            var totPontos = await palpiteRepository.ObterTotaldePontosdoApostadorNaRodada(rodadaId, apostadorId);
                            //var totPontos = apostasDoApostador.Sum(a => a.PontuacaoTotalRodada);

                            //var apostasDoApostador = await apostaRodadaRepository.ObterApostasDoApostadorNaRodada(apostadorId, rodadaId);
                            //var totPontos = apostasDoApostador.Sum(a => a.PontuacaoTotalRodada);

                            var apostaRodada = await apostaRodadaRepository.ObterApostaSalvaDoApostadorNaRodada(rodadaId, apostadorId);
                            apostaRodada.PontuacaoTotalRodada = totPontos;

                            await apostaRodadaService.Atualizar(apostaRodada);

                            var ranking = await rankingRodadaRepositoryPontuacao.ObterRankingDoApostadorNaRodada(rodadaId, apostadorId);

                            if (ranking != null)
                            {
                                ranking.DataAtualizacao = DateTime.Now;
                                ranking.Pontuacao = totPontos;
                                await rankingRodadaServicePontuacao.Atualizar(ranking);
                            }
                            else
                            {
                                _logger.LogWarning($"Ranking não encontrado para o apostador {apostadorId} na rodada {rodadaId}");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Erro ao atualizar a pontuação do ranking para o apostador {apostadorId}: {ex.Message}");
                        }
                    }

                    
                    await uowPontuacao.SaveChanges();

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro inesperado ao atualizar a pontuação do ranking: {ex}");
                    return StatusCode(500, "Ocorreu um erro inesperado ao atualizar a pontuação do ranking.");
                }
            }

            // --- Bloco para atualizar a POSIÇÃO ---
            using (var scopePosicao = _serviceScopeFactory.CreateScope())
            {
                var uowPosicao = scopePosicao.ServiceProvider.GetRequiredService<IUnitOfWork>();
                // <<-- CORREÇÃO AQUI: Obtendo o repositório do novo escopo -->>
                var rankingRodadaRepositoryPosicao = scopePosicao.ServiceProvider.GetRequiredService<IRankingRodadaRepository>() as RankingRodadaRepository;
                var rankingRodadaServicePosicao = scopePosicao.ServiceProvider.GetRequiredService<IRankingRodadaService>();

                try
                {
                    // <<-- CORREÇÃO AQUI: Usando o repositório do novo escopo -->>
                    var rankingOrdenado = await rankingRodadaRepositoryPosicao.ObterRankingDaRodadaEmOrdemDePontuacao(rodadaId);
                    var rankingListParaPosicao = rankingOrdenado.ToList();

                    var posicao = 1;
                    var pontos = -1;

                    for (int i = 0; i < rankingListParaPosicao.Count; i++)
                    {
                        if (rankingListParaPosicao[i].Pontuacao < pontos)
                        {
                            posicao++;
                        }
                        pontos = rankingListParaPosicao[i].Pontuacao;
                        rankingListParaPosicao[i].Posicao = posicao;
                        await rankingRodadaServicePosicao.Atualizar(rankingListParaPosicao[i]);
                    }

                    await uowPosicao.SaveChanges();
                    return RedirectToAction("LancarPlacaresDaRodada");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao atualizar a posição do ranking: {ex}");
                    return StatusCode(500, "Ocorreu um erro ao atualizar a posição do ranking.");
                }
            }
        }
        */





    }
}