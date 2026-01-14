
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DevBol.Presentation.ViewModels
{
    public class EquipeCampeonatoViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [DisplayName("Campeonato")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid CampeonatoId { get; set; }

        [DisplayName("Equipe")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid EquipeId { get; set; }

        public CampeonatoViewModel Campeonato { get; set; }
        public IEnumerable<CampeonatoViewModel> Campeonatos { get; set; }
        public EquipeViewModel Equipe { get; set; }

        public IEnumerable<EquipeViewModel> Equipes { get; set; }       
        

    }
}