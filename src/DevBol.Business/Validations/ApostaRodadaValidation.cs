using DevBol.Domain.Models.Equipes;
using FluentValidation;
using System.Runtime.ConstrainedExecution;

namespace DevBol.Domain.Models.Apostas
{
    public class ApostaRodadaValidation : AbstractValidator<ApostaRodada>
    {
        public ApostaRodadaValidation()
        {

            //RuleFor(a => a.JogoId)
           //    .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");

            RuleFor(a => a.ApostadorCampeonatoId)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");

            //RuleFor(a => a.DataHoraAposta)
            //    .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido");                      

        }

    }
}