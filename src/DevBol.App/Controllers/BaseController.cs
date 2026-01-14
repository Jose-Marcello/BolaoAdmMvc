using DevBol.Domain.Interfaces;
using DevBol.Domain.Notificacoes;
using Microsoft.AspNetCore.Mvc;

namespace DevBol.Presentation.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly INotificador _notificador;

        protected Guid UserId { get; set; }
        protected string UserName { get; set; }

        //protected BaseController(INotificador notificador,
        //                         IAppIdentityUser user)
        protected BaseController(INotificador notificador)
        {
            _notificador = notificador;

            //if (user.IsAuthenticated())
            //{
            //    UserId = user.GetUserId();
            //    UserName = user.GetUsername();
            //}
        }

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }
    }
}


/*

using DevBol.Domain.Interfaces;
using DevBol.Domain.Notificacoes;
using Microsoft.AspNetCore.Mvc;

namespace DevBol.Presentation.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly INotificador _notificador; // Alterado para protected

        protected Guid UserId { get; set; }
        protected string UserName { get; set; }

        protected BaseController(INotificador notificador)
        {
            _notificador = notificador;
        }

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }

        protected void Notificar(string mensagem)
        {
            _notificador.AdicionarNotificacao(new Notificacao(mensagem));
        }

        protected IEnumerable<Notificacao> ObterNotificacoes()
        {
            return _notificador.ObterNotificacoes(); // Assumindo este método na INotificador
        }

        protected void LimparNotificacoes()
        {
            _notificador.LimparNotificacoes(); // Assumindo este método na INotificador
        }

        // Você pode ter outros métodos utilitários aqui
    }
}

  /* 



















/*

public abstract class BaseController : Controller
    {
        private readonly INotificador _notificador;

        protected Guid UserId { get; set; }
        protected string UserName { get; set; }

        //protected BaseController(INotificador notificador,
        //                         IAppIdentityUser user)
        protected BaseController(INotificador notificador)
        {
            _notificador = notificador;

            //if (user.IsAuthenticated())
            //{
            //    UserId = user.GetUserId();
            //    UserName = user.GetUsername();
            //}
        }

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }
    }
}

*/