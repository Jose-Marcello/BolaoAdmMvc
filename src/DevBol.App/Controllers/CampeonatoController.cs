using AutoMapper;
using DevBol.Presentation.ViewModels;
using DevBol.Domain.Models.Campeonatos;
using Microsoft.AspNetCore.Mvc;
using DevBol.Infrastructure.Data.Interfaces.Campeonatos;
using DevBol.Infrastructure.Data.Interfaces.Rodadas;
using DevBol.Domain.Interfaces;
using DevBol.Application.Interfaces.Campeonatos;
using DevBol.Application;
using DevBol.Domain.Models.Ufs;
using DevBol.Infrastructure.Data.Repository;
using DevBol.Infrastructure.Data.Repository.Campeonatos;
using DevBol.Domain.Models.Jogos;
using DevBol.Domain.Models.Rodadas;
using DevBol.Infrastructure.Data.Repository.Rodadas;
using DevBol.Infrastructure.Data.Repository.Jogos;
using DevBol.Infrastructure.Data.Repository.RankingRodadas;
using DevBol.Domain.Models.RankingRodadas;
using DevBol.Infrastructure.Data.Context;

namespace DevBol.Presentation.Controllers
{
    [Route("Campeonato")]
    public class CampeonatoController : BaseController
    {
        private readonly IMapper _mapper;            
        private readonly ICampeonatoService _campeonatoService;     
        private readonly IUnitOfWork _uow;
        private readonly MeuDbContext _dbContext; // Injete o DbContext



        public CampeonatoController(IMapper mapper,
                                  IUnitOfWork uow,
                                  MeuDbContext dbContext,
                                  ICampeonatoService campeonatoService,                                  
                                  INotificador notificador) : base(notificador)
                                  //IAppIdentityUser user) : base(notificador, user) 
        {
            _mapper = mapper;          
            _campeonatoService = campeonatoService;           
            _uow = uow;
            _dbContext = dbContext;


        }


        [Route("lista-de-campeonatos")]
        public async Task<IActionResult> Index()
        {
            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
            return View(_mapper.Map<IEnumerable<CampeonatoViewModel>>(await campeonatoRepository.ObterTodos()));
        }

        [Route("associar-equipes-e-apostadores-de-campeonato-ativo")]
        public async Task<IActionResult> ListarCampeonatosAtivos()
        {
            var campeonatoAtivo = await ObterCampeonatoAtivo();

            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
            var campeonatoViewModel = _mapper.Map<IEnumerable<CampeonatoViewModel>>(await campeonatoRepository.ObterListaDeCampeonatosAtivos());

            TempData["NomeCampeonato"] = campeonatoAtivo.Nome;

            return View(campeonatoViewModel);
        }             


        [Route("dados-do-campeonato/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var campeonatoViewModel = await ObterCampeonato(id);

            if (campeonatoViewModel == null)
            {
                return NotFound();
            }

            return View(campeonatoViewModel);
        }

        [Route("novo-campeonato")]        
        public IActionResult Create()       {
            
            return View("Create");
        }

        [Route("novo-campeonato")]
        [HttpPost]
        public async Task<IActionResult> Create(CampeonatoViewModel campeonatoViewModel)  
        {            
            if (!ModelState.IsValid) return View(campeonatoViewModel);

            await _campeonatoService.Adicionar(_mapper.Map<Campeonato>(campeonatoViewModel));
            
            if (!OperacaoValida()) return View(campeonatoViewModel);

            //_uow.Commit();
            await _uow.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet("editar-campeonato/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var campeonatoViewModel = await ObterCampeonatoRodadas(id);

            if (campeonatoViewModel == null)
            {
                return NotFound();
            }

            return View(campeonatoViewModel);
        }

        [HttpPost("editar-campeonato/{id:guid}")]       
        public async Task<IActionResult> Edit(Guid id, CampeonatoViewModel campeonatoViewModel)
        {
            if (id != campeonatoViewModel.Id) return NotFound();

            if (!ModelState.IsValid) return View(campeonatoViewModel);

            await _campeonatoService.Atualizar(_mapper.Map<Campeonato>(campeonatoViewModel));

            //_uow.Commit();
            await _uow.SaveChanges();

            if (!OperacaoValida()) return View(campeonatoViewModel);

            return RedirectToAction("Index");
        }

        [Route("excluir-campeonato/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var campeonato = await ObterCampeonato(id);

            if (campeonato == null)
            {
                return NotFound();
            }

            return View(campeonato);
        }

        [Route("excluir-campeonato/{id:guid}")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var campeonato = await ObterCampeonato(id);

            if (campeonato == null)
            {
                return NotFound();
            }

            await _campeonatoService.RemoverEntity(_mapper.Map<Campeonato>(campeonato));

            //_uow.Commit();
            await _uow.SaveChanges();

            if (!OperacaoValida()) return View(campeonato);

            TempData["Sucesso"] = "Campeonato excluido com sucesso!";

            return RedirectToAction("Index");
        }

        [Route("equipes-campeonato/{id:guid}")]
        public async Task<IActionResult> ListarEquipesDoCampeonato(Guid id)
        {
            var campeonato = await ObterCampeonato(id);

            if (campeonato == null)
            {
                return NotFound();           }
                       
            TempData["Campeonato"] = campeonato.Nome;

            var equipeCampeonatoViewModel = await ObterEquipesDoCampeonato(id);

            return View(equipeCampeonatoViewModel);
        }

        [Route("apostadores-campeonato/{id:guid}")]
        public async Task<IActionResult> ListarApostadoresDoCampeonato(Guid id)
        {
            var campeonato = await ObterCampeonato(id);

            if (campeonato == null)
            {
                return NotFound();
            }

            TempData["Campeonato"] = campeonato.Nome;

            var apostadorCampeonatoViewModel = await ObterApostadoresDoCampeonato(id);

            return View(apostadorCampeonatoViewModel);
        }


        [Route("atualizar-ranking-campeonato/{id:guid}")]
        public async Task<IActionResult> AtualizarRankingCampeonato(Guid id)
        {
            var campeonatoRepository = _uow.GetRepository<Campeonato>();
            var campeonato = await campeonatoRepository.ObterPorId(id);
           
            if (campeonato == null)
            {
                TempData["Erro"] = "Campeonato não encontrado.";                
                return RedirectToAction("ListarCampeonatosAtivos"); // Redirecione para a tela de gerenciamento
            }

            if (!campeonato.Ativo)
            {
                //Notificar($"O campeonato '{campeonato.Nome}' não está ativo. O ranking não pode ser atualizado.");
                TempData["Erro"] = $"O campeonato '{campeonato.Nome}' não está ativo. O ranking não pode ser atualizado.";
                return RedirectToAction("ListarCampeonatosAtivos");
            }

            // Lógica de atualização do ranking (já implementada anteriormente)
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository; ;
            var ultimaRodada = await rodadaRepository.ObterUltimaRodadaFinalizadaDoCampeonato(id);                             

            if (ultimaRodada != null)
            {
                var jogoRepository = _uow.GetRepository<Jogo>() as JogoRepository;
                var jogosNaoFinalizados = await jogoRepository.ObterJogosNaoFinalizadosNaRodada(ultimaRodada.Id);

                if (jogosNaoFinalizados.Count() > 0)
                {   
                    TempData["Erro"] = $"A última rodada (Rodada : {ultimaRodada.NumeroRodada}) do campeonato '{campeonato.Nome}' ainda possui jogos não finalizados. O ranking não será atualizado.";
                    return RedirectToAction("ListarCampeonatosAtivos");
                }
            }
            else
            {                
                TempData["Erro"] = $"Não há rodadas FINALIZADAS para o campeonato : '{campeonato.Nome}'.";
                return RedirectToAction("ListarCampeonatosAtivos");
            }

            var apostadorCampeonatoRepository = _uow.GetRepository<ApostadorCampeonato>() as ApostadorCampeonatoRepository;
            var apostadoresCampeonato = await apostadorCampeonatoRepository.ObterApostadoresDoCampeonato(id, true);
            
            //AQUI ATUALIZA TODAS AS RODADAS INCLUINDO A CORRENTE (VER SE ISSO DEVE FICAR ASSIM)
            foreach (var apostadorCampeonato in apostadoresCampeonato)
            {                
                apostadorCampeonato.Pontuacao = apostadorCampeonato.RankingRodadas.Sum(r => r.Pontuacao);                
                
                apostadorCampeonatoRepository.Atualizar(apostadorCampeonato);
            }
                       

            _dbContext.ChangeTracker.Clear(); // Limpa o Change Tracker

            var rankingParaAtualizar = await apostadorCampeonatoRepository.ObterApostadoresEmOrdemDescrescenteDePontuacao(id, true);

            int posicao = 1;
            foreach (var apostador in rankingParaAtualizar)
            {
                apostador.Posicao = posicao++;
                apostadorCampeonatoRepository.Atualizar(apostador);

            }

            //_uow.Commit();
            await _uow.SaveChanges();

            TempData["Sucesso"] = $"Ranking do campeonato '{campeonato.Nome}' atualizado com sucesso!";
            return RedirectToAction("ListarCampeonatosAtivos"); // Redirecione de volta à tela de gerenciamento
        }



        private async Task<Campeonato> ObterCampeonatoAtivo()
        {
            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
            var campeonatoAtivo = await campeonatoRepository.ObterCampeonatoAtivo();

            return campeonatoAtivo;
        }

        private async Task<IEnumerable<CampeonatoViewModel>> ObterListaDeCampeonatosAtivos()
        {
            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
            return _mapper.Map<IEnumerable<CampeonatoViewModel>>(await campeonatoRepository.ObterListaDeCampeonatosAtivos());
        }

        private async Task<CampeonatoViewModel> ObterCampeonato(Guid id)
        {
            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
            var campeonato = _mapper.Map<CampeonatoViewModel>(await campeonatoRepository.ObterCampeonato(id));
            return campeonato;
        }

        private async Task<CampeonatoViewModel> ObterCampeonatoRodadas(Guid id)
        {
            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
            return _mapper.Map<CampeonatoViewModel>(await campeonatoRepository.ObterCampeonatoRodadas(id));
        }
               

        private async Task<IEnumerable<EquipeCampeonatoViewModel>> ObterEquipesDoCampeonato(Guid id)
        {
            var equipeCampeonatoRepository = _uow.GetRepository<EquipeCampeonato>() as EquipeCampeonatoRepository;
            return _mapper.Map<IEnumerable<EquipeCampeonatoViewModel>>(await equipeCampeonatoRepository.ObterEquipesDoCampeonato(id));
        }

        private async Task<IEnumerable<ApostadorCampeonatoViewModel>> ObterApostadoresDoCampeonato(Guid id)
        {
            var apostadorCampeonatoRepository = _uow.GetRepository<ApostadorCampeonato>() as ApostadorCampeonatoRepository;
            return _mapper.Map<IEnumerable<ApostadorCampeonatoViewModel>>(await apostadorCampeonatoRepository.ObterApostadoresDoCampeonato(id));
        }


    }
}