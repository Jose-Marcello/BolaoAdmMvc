// Localização: ApostasApp.Core.Web/Controllers/ApostaRodadaController.cs (Assumindo que está na mesma pasta dos outros controllers web)


using ApostasApp.Core.Infrastructure.Data.Repository.Apostas;
using AutoMapper;
using DevBol.Application;
using DevBol.Application.Interfaces.Apostas;
using DevBol.Application.Services.Apostas;
using DevBol.Domain.Interfaces;
using DevBol.Domain.Models.Apostas;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Domain.Models.Jogos;
using DevBol.Domain.Models.RankingRodadas;
using DevBol.Domain.Models.Rodadas;
using DevBol.Infrastructure.Data.Interfaces.Apostas;
using DevBol.Infrastructure.Data.Interfaces.Rodadas;
using DevBol.Infrastructure.Data.Migrations;
using DevBol.Infrastructure.Data.Repository.Apostas;
using DevBol.Infrastructure.Data.Repository.Campeonatos;
using DevBol.Infrastructure.Data.Repository.Jogos;
using DevBol.Infrastructure.Data.Repository.RankingRodadas;
using DevBol.Infrastructure.Data.Repository.Rodadas;
using DevBol.Presentation.Controllers;
using DevBol.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevBol.Presentation.Controllers
{
    //[ApiController]
    //[Route("api/[controller]")]
    public class ApostaRodadaController : BaseController
    {
        private readonly IApostaRodadaService _apostaRodadaService;
        private readonly IPalpiteService _palpiteService;
        private readonly IPalpiteRepository _palpiteRepository;
        private readonly ILogger<ApostaRodadaController> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;


        public ApostaRodadaController(INotificador notificador,
                                     IPalpiteService palpiteService,
                                     IPalpiteRepository palpiteRepository,
                                     IApostaRodadaService apostaRodadaService,
                                     ILogger<ApostaRodadaController> logger,
                                     IUnitOfWork uow,
                                     IMapper mapper)
            : base(notificador) // Passa apenas o notificador para a BaseController
        {
            _apostaRodadaService = apostaRodadaService;
            _palpiteService = palpiteService;
            _palpiteRepository = palpiteRepository;
            _logger = logger;
            _uow = uow;
            _mapper = mapper;
        }


        [Route("gerar-apostas-da-rodada/{id:guid}")]
        public async Task<IActionResult> GerarApostasERanking(Guid Id)
        {
            // --- 1. Validação Inicial (a sua lógica de validação está correta e foi mantida) ---
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodada(Id);

            var apostaRodadaRepository = _uow.GetRepository<ApostaRodada>() as ApostaRodadaRepository;
            var temAposta = await apostaRodadaRepository.ExisteApostaNaRodada(Id);

            if (rodada == null)
            {
                TempData["Notificacao"] = "Rodada não encontrada.";
                TempData["TipoNotificacao"] = "erro";
                return RedirectToAction("ListarRodadas", "Rodada", new { });
            }

            if (temAposta)
            {
                TempData["Notificacao"] = "As Apostas desta RODADA já foram GERADAS !!";
                TempData["TipoNotificacao"] = "erro";
                return RedirectToAction("ListarRodadas", "Rodada", new { });
            }

            if (rodada.Status != StatusRodada.NaoIniciada)
            {
                TempData["Notificacao"] = "Apenas RODADAS PRONTAS podem ter as APOSTAS GERADAS!!";
                TempData["TipoNotificacao"] = "erro";
                return RedirectToAction("ListarRodadas", "Rodada", new { });
            }

            var campeonatoId = rodada.CampeonatoId;
            TempData["Campeonato"] = rodada.Campeonato.Nome;
            TempData["Rodada"] = rodada.NumeroRodada;

            var jogoRepository = _uow.GetRepository<Jogo>() as JogoRepository;
            var listaJogos = await jogoRepository.ObterJogosDaRodada(rodada.Id);

            if (listaJogos.Count() < rodada.NumJogos)
            {
                TempData["Notificacao"] = "Ainda faltam JOGOS a associar à RODADA !!";
                TempData["TipoNotificacao"] = "erro";
                return RedirectToAction("ListarRodadas", "Rodada", new { });
            }

            var apostadorCampeonatoRepository = _uow.GetRepository<Domain.Models.Campeonatos.ApostadorCampeonato>() as ApostadorCampeonatoRepository;
            var listaApostadores = await apostadorCampeonatoRepository.ObterApostadoresDoCampeonato(campeonatoId);

            // --- 2. Geração das Apostas e Palpites (Lógica Corrigida) ---
            foreach (var apostadorCampeonato in listaApostadores)
            {
                // Aposta do apostador para a rodada do campeonato
                var apostaRodada = new ApostaRodada
                {
                    Id = Guid.NewGuid(), // Gerar um novo ID para a aposta da rodada
                    RodadaId = rodada.Id,
                    ApostadorCampeonatoId = apostadorCampeonato.Id,
                    IdentificadorAposta = "#01", // Ou uma lógica para gerar um identificador único
                    DataCriacao = DateTime.Now,
                    EhApostaCampeonato = true,
                    EhApostaIsolada = false,
                    CustoPagoApostaRodada = 0,
                    PontuacaoTotalRodada = 0,
                    Enviada = false, // Não enviada até o apostador preencher e submeter
                    //Palpites = new List<Palpite>() // Inicializa a lista de palpites para esta aposta
                };

                // Para cada jogo da rodada, cria um palpite padrão e o adiciona à aposta
                foreach (var jogo in listaJogos)
                {
                    var palpite = new Palpite
                    {
                        Id = Guid.NewGuid(), // Gerar um novo ID para o palpite
                        ApostaRodadaId = apostaRodada.Id,
                        JogoId = jogo.Id,
                        PlacarApostaCasa = null, // Inicialmente nulo, pois não foi preenchido
                        PlacarApostaVisita = null, // Inicialmente nulo
                        Pontos = 0 // Inicialmente zero
                    };

                    _palpiteService.AdicionarPalpite(palpite);
                }

                // Adiciona a aposta da rodada (que já contém todos os palpites)
                //await apostaRodadaRepository.Adicionar(apostaRodada);
                await _apostaRodadaService.Adicionar(apostaRodada);

            }

            // --- 3. Geração do Ranking (Sua lógica está correta e foi mantida) ---
            var rankingRodadaRepository = _uow.GetRepository<RankingRodada>() as RankingRodadaRepository;
            foreach (var apostadorRank in listaApostadores)
            {
                var ranking = new RankingRodada
                {
                    ApostadorCampeonatoId = apostadorRank.Id,
                    RodadaId = rodada.Id,
                    DataAtualizacao = DateTime.Now,
                    Posicao = 0,
                    Pontuacao = 0
                };
                await rankingRodadaRepository.Adicionar(ranking);
            }

            // --- 4. Persistência e Finalização (Sua lógica está correta e foi ajustada para melhor prática) ---
            // Faz o commit de todas as alterações (apostas, palpites, ranking) de uma vez
            await _uow.SaveChanges();

            // Atualiza o status da rodada após o sucesso da geração de apostas
            rodada.Status = StatusRodada.EmApostas;
            await rodadaRepository.Atualizar(rodada);
            await _uow.SaveChanges();

            TempData["Notificacao"] = "Apostas e Ranking da Rodada gerados com sucesso!";
            TempData["TipoNotificacao"] = "sucesso";

            return RedirectToAction("ListarRodadas", "Rodada", null);
        }


        [Route("consultar-apostas-da-rodada/{id:guid}")]
        public async Task<IActionResult> ConsultarApostasRodada(Guid Id)
        {
            //var rodada = await ObterRodadaCorrente();

            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodada(Id);

            if (rodada == null)
            {
                return NotFound();
            }

            var campeonatoId = rodada.Campeonato.Id;

            TempData["Campeonato"] = rodada.Campeonato.Nome;
            TempData["Rodada"] = rodada.NumeroRodada;

            //var apostaRodadaRepository = _uow.GetRepository<ApostaRodada>() as ApostaRodadaRepository;
            //var apostasDaRodada = await apostaRodadaRepository.ObterApostasDaRodada(rodada.Id);

            //aqui vai ter que BUSCAR OS PALPITES DESTA APOSTARODADA
            var palpitesViewModel = _mapper.Map<IEnumerable<PalpiteViewModel>>(await _palpiteRepository.ObterPalpitesDaRodada(rodada.Id));

            if (palpitesViewModel.Count() == 0 )
            {
                TempData["Notificacao"] = "Não há APOSTAS a serem CONSULTADAS nesta RODADA !! ";
                TempData["TipoNotificacao"] = "erro"; // Opcional: para estilizar a mensagem na View   
                return RedirectToAction("ListarRodadas", "Rodada", null);
            }
                        

            // aqui pode ter uma view com todas as apostas geradas PARA CONFERÊNCIA
            return View(palpitesViewModel);

        }

        /// <summary>
        [Route("deletar-apostas-da-rodada/{id:guid}")]
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        //[HttpGet]
        public async Task<IActionResult> DeletarApostasRodadas(Guid Id)
        {

            var apostaRodadaRepository = _uow.GetRepository<ApostaRodada>() as ApostaRodadaRepository;
            var temApostaNaRodada = await apostaRodadaRepository.ExisteApostaNaRodada(Id);

            if (!temApostaNaRodada)
            {
                TempData["Notificacao"] = "Não há apostas desta RODADA a serem deletadas !! ";
                TempData["TipoNotificacao"] = "erro"; // Opcional: para estilizar a mensagem na View   
                return RedirectToAction("ListarRodadas", "Rodada", null);
            }

            var temApostaRodadaEnviada = await apostaRodadaRepository.ExisteApostaRodadaSalvaNaRodada(Id);

            if (temApostaRodadaEnviada)
            {
                TempData["Notificacao"] = "Já EXISTEM APOSTAS SALVAS (Enviadas) nesta RODADA. Ela não pode ser excluída !! ";
                TempData["TipoNotificacao"] = "erro"; // Opcional: para estilizar a mensagem na View   
                return RedirectToAction("ListarRodadas", "Rodada", null);
            }


            //var rodada = await ObterRodadaCorrente();
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = await rodadaRepository.ObterRodada(Id);

            var campeonatoId = rodada.Campeonato.Id;

            if (rodada == null)
            {
                return NotFound();
            }

            //var apostaRepository = _uow.GetRepository<Aposta>() as ApostaRepository;
            var apostasRodadas = await apostaRodadaRepository.ObterApostasDaRodada(rodada.Id);

            //deletar as apostasRodadas
            foreach (var a in apostasRodadas)
            {
                await apostaRodadaRepository.Remover(a.Id);
            }
                        
            //PALPITES
            var palpiteRepository = _uow.GetRepository<Palpite>() as PalpiteRepository;

            var palpite = await palpiteRepository.RemoverTodosPalpitesDaRodada(rodada.Id);
                        
            //deletar RANKINGRODADA
            var rankingRodadaRepository = _uow.GetRepository<RankingRodada>() as RankingRodadaRepository;

            var rank = await rankingRodadaRepository.ObterRankingRodada(rodada.Id);

            foreach (var a in rank)
            {
                await rankingRodadaRepository.Remover(a.Id);
            }

            //salva a TRANSAÇÃO - COMITANDO
            //_uow.Commit();
            await _uow.SaveChanges();

            // faz a atualização do Status da RODADA, fora da transação (devido à validação)
            rodada.Status = StatusRodada.NaoIniciada;

            await rodadaRepository.Atualizar(_mapper.Map<Rodada>(rodada));
            rodadaRepository.SaveChanges();


            TempData["Notificacao"] = "APOSTAS REMOVIDAS COM SUCESSO !! ";
            TempData["TipoNotificacao"] = "erro"; // Opcional: para estilizar a mensagem na View 
            return RedirectToAction("ListaRodadas", "Rodada", null);

            // aqui pode ter uma view sem as apostas deletadas PARA CONFERÊNCIA
            //return View(apostaViewModel);

        }

        private async Task<RodadaViewModel> ObterRodadaCorrente()
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodadaCorrente());
            return rodada;
        }


    }



}
