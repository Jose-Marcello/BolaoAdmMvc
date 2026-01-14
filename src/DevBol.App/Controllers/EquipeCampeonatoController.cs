using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DevBol.Presentation.ViewModels;
using EquipeCampeonato = DevBol.Domain.Models.Campeonatos.EquipeCampeonato;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Infrastructure.Data.Interfaces.Campeonatos;
using DevBol.Infrastructure.Data.Interfaces.Equipes;
using DevBol.Domain.Interfaces;
using DevBol.Application.Interfaces.Campeonatos;
using DevBol.Application;
using DevBol.Infrastructure.Data.Repository.Campeonatos;
using DevBol.Domain.Models.Equipes;
using DevBol.Infrastructure.Data.Repository.Equipes;

namespace DevBol.Presentation.Controllers
{
    public class EquipeCampeonatoController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly IEquipeCampeonatoService _equipeCampeonatoService;
       
        public EquipeCampeonatoController(IMapper mapper,
                                          IUnitOfWork uow, 
                                  IEquipeCampeonatoService equipeCampeonatoService,                                 
                                  INotificador notificador) : base(notificador)
        //IAppIdentityUser user) : base(notificador, user) 
        {
            _mapper = mapper;           
            _equipeCampeonatoService = equipeCampeonatoService;            
            _uow = uow;

        }

        public async Task<IActionResult> Index()
        {
            var campeonato = await ObterCampeonatoAtivo();

            var equipeCampeonatoRepository = _uow.GetRepository<EquipeCampeonato>() as EquipeCampeonatoRepository;
            return View(_mapper.Map<IEnumerable<EquipeCampeonatoViewModel>>
                        (await equipeCampeonatoRepository.ObterEquipesDoCampeonato(campeonato.Id)));
        }


        [Route("nova-equipe-campeonato")]
        public async Task<IActionResult> Create()
        {
            var equipeCampeonatoViewModel = await PopularEquipes(new EquipeCampeonatoViewModel());

            var campeonato = await ObterCampeonatoAtivo();
           
            equipeCampeonatoViewModel.Campeonato = campeonato;
            equipeCampeonatoViewModel.CampeonatoId = campeonato.Id;
            
            return View(equipeCampeonatoViewModel);
        }
       
        [Route("nova-equipe-campeonato")]
        [HttpPost]
        public async Task<IActionResult> Create(EquipeCampeonatoViewModel equipeCampeonatoViewModel)  
        {          

            if (!ModelState.IsValid) return View(equipeCampeonatoViewModel);            

            //Aqui deverá ter um serviço com validação
            await _equipeCampeonatoService.Adicionar(_mapper.Map<EquipeCampeonato>(equipeCampeonatoViewModel));

            if (!OperacaoValida()) return View(equipeCampeonatoViewModel);


            //_uow.Commit();
            await _uow.SaveChanges();

            return RedirectToAction("Index");
        }

        [Route("excluir-equipe-campeonato/{idCampeonato:guid}/{idEquipe:guid}")]
        public async Task<IActionResult> Delete(Guid idCampeonato, Guid idEquipe)
        {               

            var equipeCampeonatoViewModel = await ObterEquipeCampeonato(idCampeonato, idEquipe);

            if (equipeCampeonatoViewModel == null)
            {
                return NotFound();
            }
            var campeonato = await ObterCampeonatoAtivo();            
            var equipe = await ObterEquipe(idEquipe);
            
            equipeCampeonatoViewModel.Campeonato = campeonato;
            equipeCampeonatoViewModel.Equipe = equipe;

            return View(equipeCampeonatoViewModel);
        }

        [Route("excluir-equipe-campeonato/{idCampeonato:guid}/{idEquipe:guid}")]

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid IdCampeonato, Guid idEquipe)        {

            var equipeCampeonatoViewModel = await ObterEquipeCampeonato(IdCampeonato, idEquipe);

            if (equipeCampeonatoViewModel == null)
            {
                return NotFound();
            }

            //aqui deve ter o serviço também
            await _equipeCampeonatoService.RemoverEntity(_mapper.Map<EquipeCampeonato>(equipeCampeonatoViewModel));

            if (!OperacaoValida()) return View(equipeCampeonatoViewModel);

            //_uow.Commit();
            await _uow.SaveChanges();

            TempData["Sucesso"] = "Equipe desassociada do Campeonato com sucesso!";

            return RedirectToAction("Index");
        }


        private async Task<EquipeCampeonatoViewModel> ObterEquipeCampeonato(Guid idCampeonato, Guid idEquipe)
        {
            var equipeCampeonatoRepository = _uow.GetRepository<EquipeCampeonato>() as EquipeCampeonatoRepository;
            var equipeCampeonato = _mapper.Map<EquipeCampeonatoViewModel>
                                  (await equipeCampeonatoRepository.ObterEquipeCampeonato(idCampeonato, idEquipe));            
            return equipeCampeonato;
        }


        private async Task<CampeonatoViewModel> ObterCampeonatoAtivo()
        {
            var campeonatoRepository = _uow.GetRepository<Campeonato>() as CampeonatoRepository;
            var campeonato = _mapper.Map<CampeonatoViewModel>(await campeonatoRepository.ObterCampeonatoAtivo());

            return campeonato;
        }

        private async Task<EquipeViewModel> ObterEquipe(Guid idEquipe)
        {
            var equipeRepository = _uow.GetRepository<Equipe>() as EquipeRepository;
            var equipe = _mapper.Map<EquipeViewModel>(await equipeRepository.ObterEquipe(idEquipe));

            return equipe;
        }

        private async Task<EquipeCampeonatoViewModel> PopularEquipes(EquipeCampeonatoViewModel equipeCampeonato)
        {
            var equipeRepository = _uow.GetRepository<Equipe>() as EquipeRepository;
            equipeCampeonato.Equipes = _mapper.Map<IEnumerable<EquipeViewModel>>(await equipeRepository.ObterTodos());
            
            return equipeCampeonato;

        }

    }

}