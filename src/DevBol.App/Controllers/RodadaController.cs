using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DevBol.Presentation.ViewModels;
using DevBol.Domain.Models.Rodadas;
using DevBol.Infrastructure.Data.Interfaces.Apostas;
using DevBol.Infrastructure.Data.Interfaces.Campeonatos;
using DevBol.Infrastructure.Data.Interfaces.Jogos;
using DevBol.Infrastructure.Data.Interfaces.Rodadas;
using DevBol.Domain.Interfaces;
using DevBol.Application.Interfaces.Rodadas;
using DevBol.Application;
using DevBol.Infrastructure.Data.Repository.Campeonatos;
using DevBol.Infrastructure.Data.Repository.Rodadas;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Domain.Notificacoes;
using ApostasApp.Core.Domain.Models.Notificacoes;

namespace DevBol.Presentation.Controllers
{
    //[Route("Rodada")]
    public class RodadaController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;       
        private readonly IRodadaService _rodadaService;  
        private readonly INotificador _notificador;

        public RodadaController(IMapper mapper,
                                IUnitOfWork uow,
                                  IRodadaService rodadaService,
                                  INotificador notificador) : base(notificador)
                                  //IAppIdentityUser user) : base(notificador, user) 
        {
            _mapper = mapper;
            _uow = uow;           
            _rodadaService = rodadaService;
            _notificador = notificador;
           
        }

        //aqui acho que deverá ser alterado p/ rodadas de um campeonato 
        [Route("lista-de-rodadas")]
        public async Task<IActionResult> Index()
        {
            var campeonato = await ObterCampeonatoAtivo();

            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            return View(_mapper.Map<IEnumerable<RodadaViewModel>>
                        (await rodadaRepository.ObterRodadasDoCampeonato(campeonato.Id)));
        }

        [Route("dados-da-rodada/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var rodadaViewModel = await ObterRodada(id);

            if (rodadaViewModel == null)
            {
                return NotFound();
            }

            return View(rodadaViewModel);
        }

        [HttpGet("nova-rodada")]
        public async Task<IActionResult> Create()
        {           
            var rodadaViewModel = new RodadaViewModel();

            //aqui o Ideal será obter o campeonado selecionado
            //(deste jeito, tem que deixar o botão NOVA RODADA inabilitado para CAMPEONATOS NÃO ATIVOS)
            var campeonato = await ObterCampeonatoAtivo();

            rodadaViewModel.Campeonato = campeonato;
            rodadaViewModel.CampeonatoId = campeonato.Id;

            return View(rodadaViewModel);
        }

        [HttpPost("nova-rodada")]        
        public async Task<IActionResult> Create(RodadaViewModel rodadaViewModel)
        {
            if (!ModelState.IsValid) return View(rodadaViewModel);

          
            await _rodadaService.Adicionar(_mapper.Map<Rodada>(rodadaViewModel));

            if (!OperacaoValida()) return View(rodadaViewModel);

            //_uow.Commit();
            await _uow.SaveChanges();

            return RedirectToAction("GerenciarRodadas");
        }

        [HttpGet("editar-rodada/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var rodadaViewModel = await ObterRodada(id);

            if (rodadaViewModel == null)
            {
                return NotFound();
            }

            return View(rodadaViewModel);
        }

        [HttpPost("editar-rodada/{id:guid}")]
        //[HttpPost]
        public async Task<IActionResult> Edit(Guid id, RodadaViewModel rodadaViewModel)
        {
            if (id != rodadaViewModel.Id) return NotFound();

            if (!ModelState.IsValid) return View(rodadaViewModel);

            await _rodadaService.Atualizar(_mapper.Map<Rodada>(rodadaViewModel));

            if (!OperacaoValida())
            {
                foreach (var notificacao in _notificador.ObterNotificacoes())
                {
                    ModelState.AddModelError("", notificacao.Mensagem);
                }
                return View(rodadaViewModel);

            }

            //_uow.Commit();
            await _uow.SaveChanges();

            TempData["Sucesso"] = "Rodada atualizada com sucesso!";

            return RedirectToAction("GerenciarRodadas");
        }

        [Route("excluir-rodada/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var rodada = await ObterRodada(id);

            if (rodada == null)
            {
                return NotFound();
            }

            return View(rodada);
        }

        [Route("excluir-rodada/{id:guid}")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var rodada = await ObterRodada(id);

            if (rodada == null)
            {
                return NotFound();
            }

            await _rodadaService.Remover(id);

            if (!OperacaoValida()) return View(rodada);

            //_uow.Commit();
            await _uow.SaveChanges();

            TempData["Sucesso"] = "Rodada excluida com sucesso!";

            return RedirectToAction("GerenciarRodadas");
        }

       

        [Route("lista_de_rodadas")]
        public async Task<IActionResult> ListarRodadas()
        {
            var campeonatoAtivo = await ObterCampeonatoAtivo();

            var rodadaViewModel = await ObterListaRodadas();
            
            if (rodadaViewModel == null)
            {
                return NotFound();
            }

           
            TempData["NomeCampeonato"] = campeonatoAtivo.Nome;

            return View(rodadaViewModel);
        }

        [Route("gerenciar-rodadas-do-campeonato")]
        public async Task<IActionResult> GerenciarRodadas()
        {
            var campeonatoAtivo = await ObterCampeonatoAtivo();                         

            if (campeonatoAtivo == null)
            {
                return NotFound();
            }

            TempData["NomeCampeonato"] = campeonatoAtivo.Nome;


            var rodadaViewModel = await ObterRodadasDoCampeonato(campeonatoAtivo.Id);

            return View(rodadaViewModel);
           
        }

        [Route("selecionar_rodadas_em_construcao")]
        public async Task<IActionResult> ListarRodadasEmConstrucao()  
        {
            var campeonatoAtivo = await ObterCampeonatoAtivo();

            if (campeonatoAtivo == null)
            {
                return NotFound();
            }

            TempData["Campeonato"] = campeonatoAtivo.Nome;

            var rodadaViewModel = await ObterListaRodadasEmConstrucao();

            if (rodadaViewModel == null)
            {
                return NotFound();
            }

            return View(rodadaViewModel);
        }

        private async Task<RodadaViewModel> ObterRodada(Guid id)
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;

            var rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodadaCampeonato(id));
            rodada.Campeonatos = _mapper.Map<IEnumerable<CampeonatoViewModel>>(await campeonatoRepository.ObterTodos());
            return rodada;
        }

        private async Task<IEnumerable<RodadaViewModel>> ObterRodadasDoCampeonato(Guid id)
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            return _mapper.Map<IEnumerable<RodadaViewModel>>(await rodadaRepository.ObterRodadasDoCampeonato(id));
        }

       
        private async Task<IEnumerable<RodadaViewModel>> ObterListaRodadas()
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var listaRodada = _mapper.Map<IEnumerable<RodadaViewModel>>(await rodadaRepository.ObterRodadasCampeonato());
            return listaRodada;
        }

        private async Task<IEnumerable<RodadaViewModel>> ObterListaRodadasEmConstrucao()
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var listaRodada = _mapper.Map<IEnumerable<RodadaViewModel>>(await rodadaRepository.ObterRodadasPorStatus(StatusRodada.EmConstrucao));
            return listaRodada;
        }

        private async Task<CampeonatoViewModel> ObterCampeonatoAtivo()
        {
            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
            var campeonato = _mapper.Map<CampeonatoViewModel>(await campeonatoRepository.ObterCampeonatoAtivo());

            return campeonato;
        }

        private async Task<RodadaViewModel> PopularCampeonatos(RodadaViewModel rodada)
        {
            //implementa - TEM QUE LISTAR OS CAMPEONATOS DE UMA RODADA (no caso uma RODADA)
            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
            rodada.Campeonatos = _mapper.Map<IEnumerable<CampeonatoViewModel>>(await campeonatoRepository.ObterTodos());
            return rodada;
            //}

        }

        private async Task<RodadaViewModel> ObterRodadaCorrente()
        {
            var rodadaRepository = _uow.GetRepository<Rodada>() as RodadaRepository;
            var rodada = _mapper.Map<RodadaViewModel>(await rodadaRepository.ObterRodadaCorrente());
            return rodada;
        }

        private async Task<RodadaViewModel> AssociarCampeonato(RodadaViewModel rodada)
        {
            //TEM QUE associar o CAMPEONATO selecinado (Ativo) à RODADA 
            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
            rodada.Campeonato = _mapper.Map<CampeonatoViewModel>(await campeonatoRepository.ObterCampeonatoAtivo());
            rodada.CampeonatoId = rodada.Campeonato.Id;
            return rodada;

        }
           
        
    }


}
