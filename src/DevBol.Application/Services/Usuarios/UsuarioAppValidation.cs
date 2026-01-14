using DevBol.Application.Interfaces.Usuarios;
using FluentValidation;
using DevBol.Domain.Models.Usuarios;

public class UsuarioAppValidation : AbstractValidator<Usuario>
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioAppValidation(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;

        RuleFor(u => u.UserName)
           .MustAsync(async (userName, cancellation) =>
           {
               return !await _usuarioService.UsuarioExiste(userName);
           }).WithMessage("Este USUÁRIO já está CADASTRADO.");

        RuleFor(u => u.Apelido)
            .MustAsync(async (apelido, cancellation) =>
            {
                return !await _usuarioService.ApelidoExiste(apelido);
            }).WithMessage("Este apelido já está em uso.");


    }
}