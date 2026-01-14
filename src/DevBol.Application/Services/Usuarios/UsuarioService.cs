using DevBol.Application.Base;
using DevBol.Application.Interfaces.Usuarios;
using DevBol.Domain.Interfaces;
using DevBol.Domain.Models.Usuarios;
using DevBol.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace DevBol.Application.Services.Usuarios

{
    public class UsuarioService : BaseService, IUsuarioService
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        //private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly ISendGridClient _sendGridClient;
        //private readonly IUrlHelper _urlHelper;
        //private readonly ILogger _logger;
        //private MeuDbContext _context;
        private DbContext MeuDbContext;


        public UsuarioService(UserManager<Usuario> userManager,
                           SignInManager<Usuario> signInManager,                           
                           //LinkGenerator linkGenerator,
                           DbContextOptions<MeuDbContext> options,
                           IHttpContextAccessor httpContextAccessor,
                           INotificador notificador,
                           IUnitOfWork uow) : base(notificador, uow)
       
        {
            _userManager = userManager;
            _signInManager = signInManager;
            //_linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
            // _sendGridClient = sendGridClient;
            //_urlHelper = urlHelper;
            //_logger = logger;
        }
            
            

        //public async Task<IdentityResult> RegisterUserAsync(string email, string password)
        public async Task<IdentityResult> RegisterUserAsync(Usuario user, string password)
        {

            /*if (_uow.Usuarios.Buscar(a => a.CPF == apostador.CPF).Result.Any())           
            {
                Notificar("Já existe um apostador com este CPF informado.");
                return;
            }
            if (_uow.Usuarios.Buscar(a => a.Nome == apostador.Nome).Result.Any())            
                {
                    Notificar("Já existe um apostador com este NOME informado.");
                return;
            }
 */
            //var user = new Usuario { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password); // Declare e atribua result

            if (result.Succeeded)
            {

                // Gerar token de confirmação
                //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);


                //var confirmationLink = _linkGenerator.GetUriByAction(
                // _httpContextAccessor.HttpContext,
                //"ConfirmEmail",
                // "Account",
                //  new { userId = user.Id, token = token },
                //  _httpContextAccessor.HttpContext.Request.Scheme);

                // Enviar email de confirmação
                //await SendConfirmationEmailAsync(user.Email, confirmationLink);
            }

            return result;

        }

        /* private async Task SendConfirmationEmailAsync(string email, string confirmationLink) 
         {
             try
             {
                 var from = new EmailAddress("seu-email@seudominio.com", "Seu Nome");
                 var to = new EmailAddress(email);
                 var subject = "Confirmação de Registro";
                 var plainTextContent = "Por favor, confirme seu registro clicando no link abaixo:";

                 var htmlContent = $@"
                     <p>Por favor, confirme seu registro clicando no link abaixo:</p>
                    <a href=""{confirmationLink}"">Confirmar Registro</a>
                      ";

                 var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                 var response = await _sendGridClient.SendEmailAsync(msg);

                 // Verificar a resposta do SendGrid
                 if (!response.IsSuccessStatusCode)
                 {
                     // Lidar com erros de envio de email
                     // Registrar o erro, notificar o usuário, etc.
                     Notificar("Ocorreu um erro ao enviar o email de confirmação.");
                     return;
                 }
             }
             catch (HttpRequestException ex)
             {
                 // Lidar com problemas de conexão
                 //_logger.LogError(ex, "Erro de conexão ao enviar email de confirmação.");
                 Notificar("Ocorreu um erro de conexão ao enviar o email de confirmação.");
             }
             *//*catch (SendGridException ex)
             {
                 // Lidar com erros da API do SendGrid
                 _logger.LogError(ex, "Erro da API do SendGrid ao enviar email de confirmação.");
                 Notificar("Ocorreu um erro ao enviar o email de confirmação.");
             }*//*
             catch (Exception ex)
             {
                 // Lidar com outros erros
                // _logger.LogError(ex, "Erro inesperado ao enviar email de confirmação.");
                 Notificar("Ocorreu um erro ao enviar o email de confirmação.");
             }
         }
          */

        public async Task<Microsoft.AspNetCore.Identity.SignInResult> LoginUserAsync(string email, string password, bool rememberMe)
        {
            return await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
        }
        public async Task LogoutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(Usuario user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(Usuario user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);

        }

        public async Task<bool> IsUserInRole(Usuario user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }
        public async Task<IdentityResult> ResetPasswordAsync(Usuario user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }


        public async Task SignInAsync(Usuario user, bool isPersistent) // Implementado
        {
            await _signInManager.SignInAsync(user, isPersistent);
        }


        public async Task<Usuario> FindByEmailAsync(string email) // Implementado
        {
            return await _userManager.FindByEmailAsync(email);
        }


        public async Task<Usuario> FindByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<bool> ApelidoExiste(string apelido)
        {
            return await _userManager.Users.AnyAsync(u => u.Apelido == apelido);
        }

        public async Task<bool> UsuarioExiste(string userName)
        {
            return await _userManager.Users.AnyAsync(u => u.UserName == userName);
        }


        public async Task<Usuario> ObterUsuarioPorId(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }


        //public async Task<string> GetLoggedInUserId()
        public string GetLoggedInUserId()
        {
           
                var user =   _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User).Result;

                if (user != null)
                {
                    return user.Id.ToString();
                }

                return null;
            
        }

        public async Task<Usuario> GetLoggedInUser()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            if (user != null)
            {
                // O usuário não está autenticado
                return user;
            }

            return null;

/*
            // Obter o Id do usuário a partir das Claims
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                // O Id do usuário não foi encontrado nas Claims
                return null;
            }

            // Obter o valor do Id do usuário
            string userId = userIdClaim.Value;
            
            return user;*/

            //return await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        }


        // Outras implementações...
    }

}