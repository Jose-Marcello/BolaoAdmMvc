//using Example01.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
//using DataTables.AspNet.AspNetCore;
//using DataTables.AspNet.Core;
using System.Web.Helpers;
//using DevBol.Presentation.MockDatabase;
using System.Numerics;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Metrics;
//using System.Linq.Dynamic.Core;


using DevBol.Presentation.ViewModels;
using DevBol.Domain.Interfaces;



namespace DevBol.Presentation.Controllers
{
    public class HomeController : BaseController
    {
        //public HomeController(INotificador notificador,
        //                      IAppIdentityUser user) : base(notificador, user)

        private readonly ILogger<HomeController> _logger;

        public HomeController(INotificador notificador, ILogger<HomeController> logger) : base(notificador)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //var userName = UserName;
            //var userId = UserId;

            return View();
        }
                

        //ANTES DOS TESTES

        public IActionResult Privacy()
            {
                return View();
            }

            [Route("erro/{id:length(3,3)}")]
            public IActionResult Errors(int id)
            {
                var modelErro = new ErrorViewModel();

                if (id == 500)
                {
                    modelErro.Mensagem = "Ocorreu um erro! Tente novamente mais tarde ou contate nosso suporte.";
                    modelErro.Titulo = "Ocorreu um erro!";
                    modelErro.ErroCode = id;
                }
                else if (id == 404)
                {
                    modelErro.Mensagem = "A página que está procurando não existe! <br />Em caso de dúvidas entre em contato com nosso suporte";
                    modelErro.Titulo = "Ops! Página não encontrada.";
                    modelErro.ErroCode = id;
                }
                else if (id == 403)
                {
                    modelErro.Mensagem = "Você não tem permissão para fazer isto.";
                    modelErro.Titulo = "Acesso Negado";
                    modelErro.ErroCode = id;
                }
                else
                {
                    return StatusCode(500);
                }

                return View("Error", modelErro);
            }
        }
   }
  
