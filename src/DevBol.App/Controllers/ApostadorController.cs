using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DevBol.Presentation.ViewModels;
using DevBol.Domain.Models.Apostadores;
using DevBol.Infrastructure.Data.Interfaces.Apostadores;
using DevBol.Infrastructure.Data.Interfaces.Apostas;
using DevBol.Infrastructure.Data.Interfaces.Rodadas;
using DevBol.Domain.Interfaces;
using DevBol.Application.Interfaces.Apostadores;
using DevBol.Domain.Models.Ufs;
using DevBol.Infrastructure.Data.Repository;
using DevBol.Application;
using DevBol.Infrastructure.Data.Repository.Apostadores;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using DevBol.Domain.Models.Apostas;
using DevBol.Infrastructure.Data.Repository.Apostas;


namespace DevBol.Presentation.Controllers
{
    //[Route("Apostador")]
    public class ApostadorController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IApostadorService _apostadorService;
        private readonly IUnitOfWork _uow;


        public ApostadorController(IMapper mapper,
                                  IUnitOfWork uow,                                  
                                  IApostadorService apostadorService,                                                  
                                  INotificador notificador) : base(notificador)
                                  //IAppIdentityUser user) : base(notificador, user) 
        {
            _mapper = mapper;           
            _apostadorService = apostadorService;         
            _uow = uow;

        }
        
        [Route("lista-de-apostadores")]
        public async Task<IActionResult> Index()
        {
            var apostadorRepository = _uow.GetRepository<JogoFinalizadoEvent>() as ApostadorRepository;
            return View(_mapper.Map<IEnumerable<ApostadorViewModel>>(await apostadorRepository.ObterApostadoresEmOrdemAlfabetica()));
        }

        [Route("dados-do-apostador/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var apostadorViewModel = await ObterApostador(id);

            if (apostadorViewModel == null)
            {
                return NotFound();
            }

            return View(apostadorViewModel);
        }

      
       [Route("editar-apostador/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var apostadorViewModel = await ObterApostador(id);

            if (apostadorViewModel == null)
            {
                return NotFound();
            }

            return View(apostadorViewModel);
        }
       

        [Route("editar-apostador/{id:guid}")]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, ApostadorViewModel apostadorViewModel)
        {
            if (id != apostadorViewModel.Id) return NotFound();

            if (!ModelState.IsValid) return View(apostadorViewModel);

            await _apostadorService.Atualizar(_mapper.Map<JogoFinalizadoEvent>(apostadorViewModel));

            //_uow.Commit();
            await _uow.SaveChanges();

            if (!OperacaoValida()) return View(apostadorViewModel);

            return RedirectToAction("Index");
        }

        [Route("excluir-apostador/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var apostador = await ObterApostador(id);
                       

            if (apostador == null)
            {
                return NotFound();
            }

            var apostas = await VerificaApostasDoApostador(id);

            if (apostas == null)
            {
                return NotFound();
            }


            return View(apostador);
        }

        [Route("excluir-apostador/{id:guid}")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var apostador = await ObterApostador(id);

            if (apostador == null)
            {
                return NotFound();
            }

            await _apostadorService.RemoverEntity(_mapper.Map<JogoFinalizadoEvent>(apostador));

            //_uow.Commit();
            await _uow.SaveChanges();

            if (!OperacaoValida()) return View(apostador);

            TempData["Sucesso"] = "Apostador excluido com sucesso!";

            return RedirectToAction("Index");
        }
               
                               

        private async Task<ApostadorViewModel> ObterApostador(Guid id)
        {

            var apostadorRepository = _uow.GetRepository<JogoFinalizadoEvent>() as ApostadorRepository;
            var apostador = _mapper.Map<ApostadorViewModel>(await apostadorRepository.ObterApostador(id));
            return apostador;
        }

        private async Task<ApostaRodadaViewModel> VerificaApostasDoApostador(Guid id)
        {
            var apostaRodadaRepository = _uow.GetRepository<ApostaRodada>() as ApostaRodadaRepository;
            var apostasRodada = _mapper.Map<ApostaRodadaViewModel>(await apostaRodadaRepository.ObterApostasDoApostador(id));
            return apostasRodada;
        }



    }
}