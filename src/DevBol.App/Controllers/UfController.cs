using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using DevBol.Infrastructure.Data.Context;
using System.Linq.Dynamic.Core;
using DevBol.Domain.Interfaces;
using DevBol.Application.Interfaces.Ufs;
using DevBol.Domain.Interfaces.Ufs;
using DevBol.Application;
using DevBol.Domain.Models.Ufs;
using DevBol.Infrastructure.Data.Repository;


namespace DevBol.Presentation.Controllers
{

    //[Route("api/[controller]")]
    //[ApiController]
    public class UfController : BaseController
    {
       
        private readonly IUfService _ufService;
        private readonly IMapper _mapper;
        private readonly MeuDbContext _context;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<UfController> _logger;


        public UfController(IMapper mapper,
                                     //IUfRepository ufRepository,
                                   ILogger<UfController> logger,
                                   IUnitOfWork uow,
                                   IUfService ufService,
                                   INotificador notificador,
                                   MeuDbContext context) : base(notificador)
        //IAppIdentityUser user) : base(notificador, user) 
        {
            //_ufRepository = ufRepository;
            _ufService = ufService;
            _mapper = mapper;
            _context = context;
            _uow = uow;
            _logger = logger;
        }

        //[Route("lista-de-ufs")]
        public async Task<IActionResult> Index()
        {
            return View();

        }


        [HttpGet]
        public async Task<JsonResult> ListaUfs()
        {
            var ufRepository = _uow.GetRepository<Uf>() as UfRepository;
            var listaUfs = await ufRepository.ObterUfsEmOrdemAlfabetica();

            var data = listaUfs.Select(uf => new {

                Id = uf.Id,
                Nome = uf.Nome,
                Sigla = uf.Sigla

            }).ToList();

            return Json(new { data });


        }
    }
}

