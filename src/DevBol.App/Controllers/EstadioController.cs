using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DevBol.Presentation.ViewModels;
using DevBol.Domain.Models.Estadios;
using DevBol.Presentation.Services;
using DevBol.Domain.Interfaces;
using DevBol.Application.Interfaces.Estadios;
using DevBol.Domain.Models.Ufs;
using DevBol.Infrastructure.Data.Repository;
using DevBol.Application;
using DevBol.Infrastructure.Data.Repository.Estadios;


namespace DevBol.Presentation.Controllers
{
    //[Route("Estadio")]
    public class EstadioController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly ILogger<EstadioController> _logger;      
        //private readonly IEstadioRepository _estadioRepository;    
        private readonly IEstadioService _estadioService;
        private readonly IImageUploadService _imageUploadService;
        private readonly IUnitOfWork _uow;


        public EstadioController(IMapper mapper,
                                   ILogger<EstadioController> logger,
                                   IUnitOfWork uow,
                                   IEstadioService estadioService,
                                   IImageUploadService imageUploadService,
                                  INotificador notificador) : base(notificador)
        //IAppIdentityUser user) : base(notificador, user) 
        {
            _mapper = mapper;
            _logger = logger;
            _estadioService = estadioService;
            _imageUploadService = imageUploadService;
            _uow = uow;

        }

        [HttpGet]
        [Route("lista-de-estadios")]
        public async Task<IActionResult> Index()        {

            var id = TempData["IdRegistro"];
            return View();

        }
       
        public async Task<IActionResult> BuscaEstadios()
        {
            try
            {
                // Log dos dados recebidos (mais detalhes)
                _logger.LogInformation("Dados recebidos do DataTables:");
                foreach (var key in Request.Form.Keys)
                {
                    _logger.LogInformation($"{key}: {Request.Form[key]}");
                }

                // Obter parâmetros (com tratamento de erros)
                int draw = int.TryParse(Request.Form["draw"].FirstOrDefault(), out int d) ? d : 0;
                int start = int.TryParse(Request.Form["start"].FirstOrDefault(), out int s) ? s : 0;
                int length = int.TryParse(Request.Form["length"].FirstOrDefault(), out int l) ? l : 0;
                               
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
               
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
               
                // Paging Size (10, 20, 50, 100)               
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
               
                int skip = start != null ? Convert.ToInt32(start) : 0;
               
                int recordsTotal = 0;

                // CARREGA OS ESTÁDIOS
                var estadioRepository = _uow.GetRepository<Estadio>() as EstadioRepository;
                var estadioData = await estadioRepository.ObterEstadiosUf();

                // Pesquisa
                if (!string.IsNullOrEmpty(searchValue))
                {
                    estadioData = estadioData.Where(m => m.Nome.Contains(searchValue)); // Usar Contains para pesquisa parcial
                }                              

                recordsTotal = estadioData.Count();

                // Paginação
                var data = estadioData.Skip(start).Take(length).ToList();
                

                //ORDENAÇÃO - REVISAR
                if (!string.IsNullOrEmpty(sortColumn))
                {
                    if (sortColumnDirection == "asc")
                    {
                        data = data.OrderBy(e => e.GetType().GetProperty(sortColumn)?.GetValue(e)).ToList();
                    }
                    else
                    {
                        data = data.OrderByDescending(e => e.GetType().GetProperty(sortColumn)?.GetValue(e)).ToList();
                    }
                }
               

                // **CORREÇÃO:** Converter para um formato anônimo antes de serializar para JSON
                var formattedData = data.Select(estadio => new
                {

                    Id = estadio.Id,
                    Nome = estadio.Nome,

                    Uf = new
                    {
                        Id = estadio.Uf.Id,
                        Nome = estadio.Uf.Nome,
                        Sigla = estadio.Uf.Sigla

                    }

                }).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = formattedData });

            }
          

            catch (Exception)
            {
                throw;
            }
        }                             

        // Função auxiliar para inverter strings (para ordenação descendente)
        private string InvertString(string s)
        {
            if (s == null) return null;
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }


        [Route("novo-estadio")]
        public async Task<IActionResult> Create()
        {
            var estadioViewModel = await PopularUfs(new EstadioViewModel());

            return View(estadioViewModel);
        }

        [Route("novo-estadio")]
        [HttpPost]
        public async Task<IActionResult> Create(EstadioViewModel estadioViewModel)
        {

            if (!ModelState.IsValid) return View(estadioViewModel);

            await _estadioService.Adicionar(_mapper.Map<Estadio>(estadioViewModel));

            //_uow.Commit();
            await _uow.SaveChanges();

            if (!OperacaoValida()) return View(estadioViewModel);

            return RedirectToAction("Index");
        }

        private async Task<EstadioViewModel> PopularUfs(EstadioViewModel estadio)
        {
            var ufRepository = _uow.GetRepository<Uf>() as UfRepository;
            estadio.Ufs = _mapper.Map<IEnumerable<UfViewModel>>(await ufRepository.ObterUfsEmOrdemAlfabetica());
            return estadio;

        }

        //[Route("editar-estadio/{id:guid}")]
        // GET: DemoGrid/Edit/5
        [Route("Estadio/Edit/{id}")]

        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id)
        {
            var estadioViewModel = await ObterEstadio(id);

            if (estadioViewModel == null) // <-- Verifique se o resultado é nulo
            {
                return NotFound(); // Ou outra ação apropriada, como redirecionar para uma página de erro
            }

            TempData["IdRegistro"] = estadioViewModel.Id;

            estadioViewModel = await PopularUfs(estadioViewModel);

            if (estadioViewModel == null)
            {
                return NotFound();
            }

            return View(estadioViewModel);
        }

        //[Route("editar-estadio/{id:guid}")]
        [Route("Estadio/Edit/{id}")]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, EstadioViewModel estadioViewModel)
        {

            estadioViewModel = await PopularUfs(estadioViewModel);

            if (id != estadioViewModel.Id) return NotFound();

            //Busca a equipe, para pegar a imagem original do escudo e caso não altere, não perca o valor gravado
            var estadioDb = await ObterEstadio(id);

            if (estadioViewModel == null) // <-- Verifique se o resultado é nulo
            {
                return NotFound(); // Ou outra ação apropriada, como redirecionar para uma página de erro
            }

            if (!ModelState.IsValid) return View(estadioViewModel);            
           

            await _estadioService.Atualizar(_mapper.Map<Estadio>(estadioViewModel));

            //_uow.Commit();
            // await _uow.SaveChanges();


            if (!OperacaoValida()) return View(estadioViewModel);

            //return RedirectToAction("Index");

            return RedirectToAction("Index");

        }
     
        //[Route("excluir-estadio/{id:guid}")]
        [Route("Estadio/Delete/{id}")]
        //[HttpPost, ActionName("Delete")]
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var estadio = await ObterEstadio(id);

            if (estadio == null)
            {
                return NotFound();
            }

            //await _estadioService.RemoverEntity(_mapper.Map<Estadio>(estadio));

            await _estadioService.Remover(estadio.Id);

            try
            {
                //_uow.Commit();                
                await _uow.SaveChanges();

                TempData["Sucesso"] = "Estádio excluído com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Logar o erro (importante!)
                //_uow.Rollback(); // Em caso de erro no Commit, fazer rollback
                ModelState.AddModelError(string.Empty, "Ocorreu um erro ao excluir o estádio.");
                // Você pode optar por redirecionar de volta para a página de confirmação com a mensagem de erro
                return View("Delete", _mapper.Map<EstadioViewModel>(estadio));
            }
                        
        }


        private async Task<EstadioViewModel> ObterEstadio(Guid id)
        {
            var estadioRepository = _uow.GetRepository<Estadio>() as EstadioRepository;
            var estadio = _mapper.Map<EstadioViewModel>(await estadioRepository.ObterEstadio(id));                      
            return estadio;
        }

    }
}

