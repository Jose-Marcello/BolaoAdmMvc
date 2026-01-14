using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DevBol.Presentation.ViewModels;
using DevBol.Domain.Models.Equipes;
using DevBol.Presentation.Services;
using Microsoft.EntityFrameworkCore;
using DevBol.Domain.Models.Rodadas;
using DevBol.Domain.Models.Estadios;
using DevBol.Infrastructure.Data.Interfaces.Equipes;
using DevBol.Domain.Interfaces;
using DevBol.Application.Interfaces.Equipes;
using DevBol.Domain.Interfaces.Ufs;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Infrastructure.Data.Repository.Campeonatos;
using DevBol.Infrastructure.Data.Repository.Equipes;
using DevBol.Application;
using DevBol.Infrastructure.Data.Repository;
using DevBol.Domain.Models.Ufs;

namespace DevBol.Presentation.Controllers
{
    //[Route("Equipe")]
    public class EquipeController : BaseController
    {
        private readonly IMapper _mapper;        
        private readonly ILogger<EquipeController> _logger;       
        private readonly IEquipeService _equipeService;
        private readonly IImageUploadService _imageUploadService;
        private readonly IUnitOfWork _uow;

        //private readonly IJogoRepository _jogoRepository;


        public EquipeController(IMapper mapper,
                                IUnitOfWork uow,
                                 ILogger<EquipeController> logger,
                                 IEquipeService equipeService,                                
                                 IImageUploadService imageUploadService,
                                 INotificador notificador) : base(notificador)
                                  //IAppIdentityUser user) : base(notificador, user) 
        {
            _mapper = mapper;
            _logger = logger;             
            _equipeService = equipeService;        
            _imageUploadService = imageUploadService;
            _uow = uow;


            //_jogoRepository = jogoRepository;
        }

        //aqui acho que deverá ser alterado p/ rodadas de um campeonato 
        [Route("lista-de-equipes")]
        public async Task<IActionResult> Index()
        {            
            return View();
        }

      
        //datatables com Get - Processamento no cliente
        [HttpGet]
        public async Task<JsonResult> ListaEquipes()
        {
            var equipeRepository = _uow.GetRepository<Equipe>() as EquipeRepository;
            var listaEquipes = await equipeRepository.ObterEquipesUf();

            var data = listaEquipes.Select(equipe => new {

                Id = equipe.Id,
                Nome = equipe.Nome,
                Sigla = equipe.Sigla,
                Tipo = equipe.Tipo.ToString(), 
                Escudo = equipe.Escudo,
                
                Uf = new
                {
                    Id = equipe.Uf.Id,
                    Nome = equipe.Uf.Nome,
                    Sigla = equipe.Uf.Sigla

                }

            }).ToList();


            //var listaOrdenada = data.Order();

            return Json(new { data });


        }
    

        [Route("dados-da-equipe/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var equipeViewModel = await ObterEquipe(id);

            if (equipeViewModel == null)
            {
                return NotFound();
            }

            return View(equipeViewModel);
        }

        [Route("nova-equipe")]        
        public async Task<IActionResult> Create()
        {
            var equipeViewModel = await PopularUfs(new EquipeViewModel());

            return View(equipeViewModel);            
        }

        [Route("nova-equipe")]
        [HttpPost]
        public async Task<IActionResult> Create(EquipeViewModel equipeViewModel)
        {            

            if (!ModelState.IsValid) return View(equipeViewModel);

            var imgPrefixo = Guid.NewGuid() + "_";

            if (!await _imageUploadService.UploadArquivo(ModelState, equipeViewModel.EscudoUpload, imgPrefixo))
            {
                return View(equipeViewModel);
            }

            equipeViewModel.Escudo = imgPrefixo + equipeViewModel.EscudoUpload.FileName;

            await _equipeService.Adicionar(_mapper.Map<Equipe>(equipeViewModel));

            if (!OperacaoValida()) return View(equipeViewModel);

            //_uow.Commit();
            await _uow.SaveChanges();

            return RedirectToAction("Index");
        }

        //[Route("editar-equipe/{id:guid}")]
        [HttpGet("Equipe/Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var equipeViewModel = await ObterEquipe(id);

            equipeViewModel = await PopularUfs(equipeViewModel); 

            if (equipeViewModel == null)
            {
                return NotFound();
            }

            return View(equipeViewModel);
        }

        //[Route("editar-equipe/{id:guid}")]
        [HttpPost("Equipe/Edit/{id}")]        
        public async Task<IActionResult> Edit(Guid id, EquipeViewModel equipeViewModel)
        {
            if (id != equipeViewModel.Id) return NotFound();


            //Busca a equipe, para pegar a imagem original do escudo e caso não altere, não perca o valor gravado
            var equipeDb = await ObterEquipe(id);

            if (!ModelState.IsValid) return View(equipeViewModel);

            equipeViewModel.Escudo = equipeDb.Escudo;

            //se carregou uma nova imagem
            if (equipeViewModel.EscudoUpload != null)
            {
                var imgPrefixo = Guid.NewGuid() + "_";
                if (!await _imageUploadService.UploadArquivo(ModelState, equipeViewModel.EscudoUpload, imgPrefixo))
                {
                    return View(equipeViewModel);
                }

                equipeViewModel.Escudo = imgPrefixo + equipeViewModel.EscudoUpload.FileName;
            }


            await _equipeService.Atualizar(_mapper.Map<Equipe>(equipeViewModel));

            //_uow.Commit();
            await _uow.SaveChanges();

            //var equipeRepository = _uow.GetRepository<Equipe>() as EquipeRepository;
            //await equipeRepository.SaveChanges();


            if (!OperacaoValida()) return View(equipeViewModel);

            return RedirectToAction("Index");
        }


     
        [HttpPost("Equipe/Delete/{id}")]      
        public async Task<IActionResult> Delete(Guid id)
        {
            var equipe = await ObterEquipe(id);

            if (equipe == null)
            {
                return NotFound();
            }

            await _equipeService.RemoverEntity(_mapper.Map<Equipe>(equipe));

            //_uow.Commit();
            await _uow.SaveChanges();


            if (!OperacaoValida()) return View(equipe);

            TempData["Sucesso"] = "Equipe excluida com sucesso!";

            return RedirectToAction("Index");
        }

 
        private async Task<EquipeViewModel> ObterEquipe(Guid id)
        {
            var equipeRepository = _uow.GetRepository<Equipe>() as EquipeRepository;
            var equipe = _mapper.Map<EquipeViewModel>(await equipeRepository.ObterEquipe(id));         
            return equipe;
        }

        private async Task<EquipeViewModel> PopularUfs(EquipeViewModel equipe)        {

            var ufRepository = _uow.GetRepository<Uf>() as UfRepository;
            equipe.Ufs = _mapper.Map<IEnumerable<UfViewModel>>(await ufRepository.ObterUfsEmOrdemAlfabetica());
            return equipe;           

        }
    }
}

