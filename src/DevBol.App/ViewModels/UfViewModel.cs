using DevBol.Presentation.Extensions;
using DevBol.Domain.Models.Campeonatos;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevBol.Presentation.ViewModels
{
    public class UfViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [DisplayName("Nome da UF")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Nome { get; set; }

        [DisplayName("Sigla da UF")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Sigla { get; set; }

        public IEnumerable<EquipeViewModel> Equipes { get; set; }
        public IEnumerable<EstadioViewModel> Estadios { get; set; }

    }
}