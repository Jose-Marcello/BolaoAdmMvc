using DevBol.Domain.Models.Usuarios;
using Microsoft.AspNetCore.Identity;

namespace DevBol.Application.Interfaces.Usuarios
{
    public interface IUsuarioService
    {

        Task<IdentityResult> RegisterUserAsync(Usuario user, string password);

        Task<Microsoft.AspNetCore.Identity.SignInResult> LoginUserAsync(string email, string password, bool rememberMe);

        Task LogoutUserAsync();

        Task SignInAsync(Usuario user, bool isPersistent); // Adicionado

        Task<Usuario> FindByEmailAsync(string email); // Adicionado

        Task<string> GenerateEmailConfirmationTokenAsync(Usuario user);

        Task<string> GeneratePasswordResetTokenAsync(Usuario user);

        Task<IdentityResult> ResetPasswordAsync(Usuario user, string token, string newPassword);

        Task<Usuario> FindByIdAsync(string id);

        Task<bool> ApelidoExiste(string apelido);

        Task<bool> UsuarioExiste(string userName);

        Task<Usuario> ObterUsuarioPorId(string id);

        Task<bool> IsUserInRole(Usuario user, string roleName);

        Task<Usuario> GetLoggedInUser();

        string GetLoggedInUserId();



        // Outras operações...// Outras operações...


    }


}