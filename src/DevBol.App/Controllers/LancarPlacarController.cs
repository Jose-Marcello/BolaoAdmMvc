using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DevBol.Presentation.ViewModels;
using DevBol.Application.Interfaces.Apostadores;
using DevBol.Domain.Models.Campeonatos;
using System.Collections.Generic;
using System.Collections;
using NuGet.Protocol;
using System.Data;
using DevBol.Domain.Models.Rodadas;
using DevBol.Domain.Models.Jogos;
using DevBol.Application.Services.Jogos;
using DevBol.Infrastructure.Data.Repository.Campeonatos;
using Microsoft.EntityFrameworkCore;
using DevBol.Domain.Models.RankingRodadas;
using DevBol.Infrastructure.Data.Repository;
using Microsoft.Extensions.DependencyInjection;
using DevBol.Infrastructure.Data.Interfaces.Apostas;
using DevBol.Infrastructure.Data.Interfaces.Campeonatos;
using DevBol.Infrastructure.Data.Interfaces.Jogos;
using DevBol.Infrastructure.Data.Interfaces.RankingRodadas;
using DevBol.Infrastructure.Data.Interfaces.Rodadas;
using DevBol.Domain.Interfaces;
using DevBol.Application.Interfaces.RankingRodadas;
using DevBol.Application;
using DevBol.Application.Interfaces.Jogos;
using DevBol.Application.Interfaces.Apostas;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using DevBol.Infrastructure.Data.Repository.Apostas;
using DevBol.Infrastructure.Data.Repository.RankingRodadas;
using DevBol.Infrastructure.Data.Repository.Rodadas;
using DevBol.Infrastructure.Data.Repository.Jogos;
using DevBol.Domain.Models.Apostas;
using DevBol.Application.Services.Apostas;
using ApostasApp.Core.Infrastructure.Data.Repository.Apostas; // Adicionado para ILogger
using DevBol.Application.BackGroundsServices;

namespace DevBol.Presentation.Controllers
{
    [Route("LancarPlacar")]
    public class LancarPlacarController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IJogoService _jogoService;
        private readonly IPalpiteService _palpiteService;
        private readonly IApostaRodadaService _apostaRodadaService;
        private readonly IApostaRodadaRepository _apostaRodadaRepository;
        private readonly IPalpiteRepository _palpiteRepository;
        private readonly IRankingRodadaService _rankingRodadaService;
        private readonly ILogger<LancarPlacarController> _logger;
        private readonly IUnitOfWork _uow;
        private readonly LancamentoPlacarQueue _placarQueue;

        public LancarPlacarController(IMapper mapper,
                                      IUnitOfWork uow,
                                      IJogoService jogoService,
                                      IPalpiteService palpiteService,
                                      IApostaRodadaService apostaRodadaService,
                                      IApostaRodadaRepository apostaRodadaRepository,
                                      IPalpiteRepository palpiteRepository,
                                      IRankingRodadaService rankingRodadaService,
                                      ILogger<LancarPlacarController> logger,
                                      IServiceScopeFactory serviceScopeFactory,
                                      LancamentoPlacarQueue placarQueue,
                                      INotificador notificador) : base(notificador)
        {
            _mapper = mapper;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _uow = uow;
            _jogoService = jogoService;
            _palpiteService = palpiteService;
            _palpiteRepository = palpiteRepository;
            _apostaRodadaService = apostaRodadaService;
            _apostaRodadaRepository = apostaRodadaRepository;
            _rankingRodadaService = rankingRodadaService;
            _placarQueue = placarQueue;
        }
            

        [Route("lancar-placares-da-rodada-corrente")]
        public async Task<IActionResult> LancarPlacaresDaRodada()
        {
            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
            var campeonato = await campeonatoRepository.ObterCampeonatoAtivo();

            var rodada = await ObterRodadaCorrente();

            try
            {
                if (rodada == null)
                {
                    ViewBag.Mensagem = "Não há rodada corrente disponível.";
                    return View(new List<JogoViewModel>());
                }

                TempData["Campeonato"] = campeonato.Nome;
                TempData["Rodada"] = rodada.NumeroRodada;
                TempData["RodadaId"] = rodada.Id;
                TempData["NumJogos"] = rodada.NumJogos;
                TempData["Status"] = rodada.Status;

                var jogoViewModel = await ObterJogosdaRodada(rodada.Id);

                return View(jogoViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao carregar placares da rodada: {ex.Message}");
                return View(); // Retorna view vazia em caso de erro. Considere adicionar uma mensagem de erro na view.
            }
        }

        [Route("editar-placar-jogo/{id:guid}")]
        public async Task<IActionResult> EditarJogo(Guid id)
        {
            var jogoViewModel = await ObterJogoEstadioEquipes(id);

            if (jogoViewModel == null)
            {
                return NotFound();
            }

            return View(jogoViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SalvarJogo([Bind("Id,EquipeCasaId,EquipeVisitanteId,RodadaId,EstadioId,DataJogo,HoraJogo,PlacarCasa,PlacarVisita,Status")] JogoViewModel jogoViewModel)
        {
            try
            {
                if (!ModelState.IsValid) return View("EditarJogo", jogoViewModel);

                var jogoRepository = _uow.GetRepository<Jogo>() as JogoRepository;
                var jogoCorrente = await jogoRepository.ObterJogo(jogoViewModel.Id);

                if (jogoCorrente == null)
                {
                    ModelState.AddModelError("", "Jogo não encontrado.");
                    return View("EditarJogo", jogoViewModel);
                }

                // Validações de Status e Placar (Mantemos aqui para dar feedback imediato ao Admin)
                if (jogoCorrente.Status == StatusJogo.NaoIniciado && (jogoViewModel.PlacarCasa != 0 || jogoViewModel.PlacarVisita != 0))
                {
                    ModelState.AddModelError("Placar", "O placar deve ser nulo para jogos não iniciados.");
                    return View("EditarJogo", jogoViewModel);
                }

                // Atualiza apenas os dados básicos do Jogo
                jogoCorrente.PlacarCasa = jogoViewModel.PlacarCasa;
                jogoCorrente.PlacarVisita = jogoViewModel.PlacarVisita;
                jogoCorrente.Status = jogoViewModel.Status;

                await _jogoService.Atualizar(jogoCorrente);

                // COMMIT DA ETAPA SÍNCRONA: Salva o placar.
                // Se falhar aqui, nem vai para a fila.
                await _uow.SaveChanges();

                // <<-- A MÁGICA DA ESCALA -->>
                // Enviamos o ID do JOGO (não da rodada, pois o Worker precisa saber qual jogo disparou o recalculo)
                // O Worker vai: 1. Pegar esse jogo, 2. Fazer o loop dos 500+ palpites, 3. Atualizar Ranking.
                await _placarQueue.EscreverRodadaAsync(jogoCorrente.Id);

                return RedirectToAction("LancarPlacaresDaRodada");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao salvar jogo: {ex.Message}");
                return View("EditarJogo", jogoViewModel);
            }
        }

        
        private async Task<RodadaViewModel> ObterRodadaCorrente()
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodadaCorrente());
            return rodada;
        }

        private async Task<IEnumerable<JogoViewModel>> ObterJogosdaRodada(Guid id)
        {
            var jogoRepository = _uow.GetRepository<Jogo>() as JogoRepository;
            var jogo = _mapper.Map<IEnumerable<JogoViewModel>>(await jogoRepository.ObterJogosDaRodada(id));
            return jogo;
        }

        private async Task<JogoViewModel> ObterJogoRodada(Guid id)
        {
            var jogoRepository = _uow.GetRepository<Jogo>() as JogoRepository;
            var jogo = _mapper.Map<JogoViewModel>(await jogoRepository.ObterJogoRodada(id));
            return jogo;
        }

        private async Task<JogoViewModel> ObterJogoEstadioEquipes(Guid id)
        {
            var jogoRepository = _uow.GetRepository<Jogo>() as JogoRepository;
            var jogo = _mapper.Map<JogoViewModel>(await jogoRepository.ObterJogoEstadioEquipes(id));
            return jogo;
        }
    }
}