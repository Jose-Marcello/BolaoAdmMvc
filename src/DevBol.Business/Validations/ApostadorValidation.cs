using DevBol.Domain.Models.Apostadores;
using DevBol.Domain.Validations.Documentos;
using FluentValidation;

namespace DevBol.Domain.Validations
{
    public class ApostadorValidation : AbstractValidator<JogoFinalizadoEvent>
    {
        public ApostadorValidation()
        {
           /* RuleFor(a => a.Nome)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .Length(2, 60)
                .WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(a => a.CPF.Length).Equal(CpfValidacao.TamanhoCpf)
                .WithMessage("O campo CPF precisa ter {ComparisonValue} caracteres e foi fornecido {PropertyValue}.");

            RuleFor(a => CpfValidacao.Validar(a.CPF)).Equal(true)
                    .WithMessage("O CPF fornecido é inválido.");

            RuleFor(a => a.Celular)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .Length(11, 11)
                .WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(a => a.DataCadastro)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");
*/


        }


    }
}