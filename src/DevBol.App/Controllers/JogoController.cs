using AutoMapper;
using DevBol.Application;
using DevBol.Application.Interfaces.Jogos;
using DevBol.Application.Services;
using DevBol.Domain.Events;
using DevBol.Domain.Interfaces;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Domain.Models.Estadios;
using DevBol.Domain.Models.Jogos;
using DevBol.Domain.Models.Rodadas;
using DevBol.Infrastructure.Data.Interfaces.Rodadas;
using DevBol.Infrastructure.Data.Repository.Campeonatos;
using DevBol.Infrastructure.Data.Repository.Estadios;
using DevBol.Infrastructure.Data.Repository.Jogos;
using DevBol.Infrastructure.Data.Repository.Rodadas;
using DevBol.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace DevBol.Presentation.Controllers
{
    [Route("Jogo")]
    public class JogoController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;     
        private readonly IJogoService _jogoService;
        private readonly DevBol.Application.BackGroundsServices.RankingUpdateQueue _rankingQueue;

        public JogoController(DevBol.Application.BackGroundsServices.RankingUpdateQueue rankingQueue,
                                  IMapper mapper,
                                  IUnitOfWork uow,                                  
                                  IRodadaRepository rodadaRepository,                                 
                                  IJogoService jogoService,
                                  INotificador notificador) : base(notificador)
                                  //IAppIdentityUser user) : base(notificador, user) 
        {
            _mapper = mapper;           
            _jogoService = jogoService;
            _uow = uow;
            _rankingQueue = rankingQueue;
        }

         
        [Route("lista-de-jogos")]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [Route("dados-do-jogo/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var jogoViewModel = await ObterJogo(id);


            if (jogoViewModel == null)
            {
                return NotFound();
            }            

            return View(jogoViewModel);
        }

        //[HttpGet("novo-jogo/{id:guid}")]
        [HttpGet("Create/{id:guid}")]
        public async Task<IActionResult> Create()
        {
            if (!Guid.TryParse(Request.RouteValues["id"]?.ToString(), out Guid idRodada))
            {
                // Se o id da rodada não for um GUID válido ou não estiver na rota,
                // você precisa lidar com esse erro (ex: redirecionar ou exibir uma mensagem).
                return BadRequest("ID da Rodada inválido ou não fornecido na rota.");
            }

            var jogoViewModel = await PopularEstadios(new JogoViewModel());

            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodada(idRodada));

            if (rodada == null)
            {
                return NotFound("Rodada não encontrada.");
            }

            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
            var campeonato = _mapper.Map<CampeonatoViewModel>(await campeonatoRepository.ObterCampeonatoAtivo());

            jogoViewModel = await PopularEquipesDoCampeonato(jogoViewModel, campeonato.Id);

            jogoViewModel.Rodada = rodada;
            jogoViewModel.RodadaId = rodada.Id;
            jogoViewModel.DataJogo = rodada.DataInic; // inicializando a data do jogo com a data inicial da rodada

            return View(jogoViewModel);
        }



        [HttpPost("Create/{RodadaId}")]
        public async Task<IActionResult> Create(Guid RodadaId, JogoViewModel jogoViewModel)
        {
            if (!ModelState.IsValid) return View(jogoViewModel);

            await _jogoService.Adicionar(_mapper.Map<Jogo>(jogoViewModel));

            if (!OperacaoValida()) return View(jogoViewModel);

            //_uow.Commit();
            await _uow.SaveChanges();

            return RedirectToAction("ManterJogos", "Jogo", new { id = RodadaId });
        }

        
        [HttpGet("editar-jogo/{id:guid}")]
        [ActionName("Edit")]       
        public async Task<IActionResult> Edit(Guid id)
        {
            var jogoViewModel = await ObterJogo(id);

            jogoViewModel = await PopularEstadios(jogoViewModel);
            
            jogoViewModel = await PopularEquipesDoCampeonato(jogoViewModel, jogoViewModel.Rodada.CampeonatoId);
                       
         
            if (jogoViewModel == null)
            {
                return NotFound();
            }

            
            return View(jogoViewModel);
        }

        [HttpPost("editar-jogo/{id:guid}")]
        [ActionName("Edit")]        
        public async Task<IActionResult> Edit(Guid id, JogoViewModel jogoViewModel)
        {
                        
            if (id != jogoViewModel.Id) return NotFound();

            if (!ModelState.IsValid) return View(jogoViewModel);

            await _jogoService.Atualizar(_mapper.Map<Jogo>(jogoViewModel));

            var jogoRepository = _uow.GetRepository<Jogo>() as JogoRepository;                     

            jogoViewModel = await ObterJogo(id);

            jogoViewModel = await PopularEstadios(jogoViewModel);

            jogoViewModel = await PopularEquipesDoCampeonato(jogoViewModel, jogoViewModel.Rodada.CampeonatoId);

            if (!OperacaoValida()) return View(jogoViewModel);

            //_uow.Commit();
            await _uow.SaveChanges();

            // -------------------------------------------------------
            // 3. O PULO DO GATO: DISPARAR O EVENTO
            // Só chegamos aqui se o banco salvou com sucesso.
            // Agora avisamos a fila que este jogo precisa ter seus pontos calculados.
            // -------------------------------------------------------
            await _rankingQueue.EscreverJogoAsync(id);


            return RedirectToAction("ManterJogos");
        }

        [Route("excluir-jogo/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var jogo = await ObterJogo(id);

            if (jogo == null)
            {
                return NotFound();
            }

            return View(jogo);
        }

        [Route("excluir-jogo/{id:guid}")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var jogo = await ObterJogo(id);

            if (jogo == null)
            {
                return NotFound();
            }

            await _jogoService.Remover(id);

            if (!OperacaoValida()) return View(jogo);

            //_uow.Commit();
            await _uow.SaveChanges();


            TempData["Sucesso"] = "Jogo excluido com sucesso!";

            return RedirectToAction("ManterJogos");
        }

        [Route("jogos-da_rodada-selecionada/{Id:guid}")]
        public async Task<IActionResult> ManterJogos(Guid Id)
        {
            //aqui pensar numa alteração: Permitir o gerenciamento de todas as RODADAS EM CONSTRUÇÃO ...
                        
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodada(Id));
            
            //var campeonato = rodada.Campeonato;

            if (rodada == null)
            {
                return NotFound();
            }

            TempData["NomeCampeonato"] = rodada.Campeonato.Nome;
            TempData["Rodada"] = rodada.NumeroRodada;
            TempData["NumJogos"] = rodada.NumJogos;
                        
            ViewBag.rodadaId = rodada.Id;

            var jogoViewModel = await ObterJogosdaRodada(Id);

            TempData["Lancados"] = jogoViewModel.Count();

            return View(jogoViewModel);
        }

        private async Task<JogoViewModel> ObterJogo(Guid id)
        {
            var jogoRepository = _uow.GetRepository<Jogo>() as JogoRepository;
            var jogo = _mapper.Map<JogoViewModel>(await jogoRepository.ObterJogoRodada(id));

            var equipeCampeonatoRepository = _uow.GetRepository<EquipeCampeonato>() as EquipeCampeonatoRepository;
            jogo.EquipesCampeonato = _mapper.Map<IEnumerable<EquipeCampeonatoViewModel>>(await equipeCampeonatoRepository.ObterEquipesDoCampeonato(jogo.Rodada.CampeonatoId));

            var estadioRepository = _uow.GetRepository<Estadio>() as EstadioRepository;
            jogo.Estadios = _mapper.Map<IEnumerable<EstadioViewModel>>(await estadioRepository.ObterEstadiosEmOrdemAlfabetica());
            return jogo;
        }


        private async Task<RodadaViewModel> ObterRodadaCorrente()
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodadaCorrente());
            return rodada;
        }

        private async Task<RodadaViewModel> ObterRodadaNaoIniciada()
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodadaProntaNaoIniciada());
            return rodada;
        }
        private async Task<IEnumerable<JogoViewModel>> ObterJogosdaRodada(Guid id)
        {
            var jogoRepository = _uow.GetRepository<Jogo>() as JogoRepository;
            var jogo = _mapper.Map<IEnumerable<JogoViewModel>>(await jogoRepository.ObterJogosDaRodada(id));            
            return jogo;
        }
        private async Task<CampeonatoViewModel> ObterCampeonato(Guid id)
        {
            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
            var campeonato = _mapper.Map<CampeonatoViewModel>(await campeonatoRepository.ObterCampeonato(id));            
            return campeonato;
        }


        private async Task<RodadaViewModel> ObterRodadaAtiva()
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodadaCorrente());

            return rodada;
        }
                       

        private async Task<JogoViewModel> AssociarRodada(JogoViewModel jogo)
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            jogo.Rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodadaCorrente());
            jogo.RodadaId = jogo.Rodada.Id;
            return jogo;

        }

        private async Task<JogoViewModel> PopularEstadios(JogoViewModel jogo)
        {
          
            var estadioRepository = _uow.GetRepository<Estadio>() as EstadioRepository;
            jogo.Estadios = _mapper.Map<IEnumerable<EstadioViewModel>>(await estadioRepository.ObterEstadiosEmOrdemAlfabetica());
            return jogo;    
        }

        private async Task<JogoViewModel> PopularEquipesDoCampeonato(JogoViewModel jogo, Guid campeonatoId)
        {
            var equipeCampeonatoRepository = _uow.GetRepository<EquipeCampeonato>() as EquipeCampeonatoRepository;
            jogo.EquipesCampeonato = _mapper.Map<IEnumerable<EquipeCampeonatoViewModel>>(await equipeCampeonatoRepository.ObterEquipesDoCampeonato(campeonatoId));
            return jogo;
        }

    }
}
