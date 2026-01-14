using DevBol.Presentation.Extensions;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Domain.Models.Estadios;
using DevBol.Domain.Models.Jogos;
using DevBol.Domain.Models.Rodadas;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace DevBol.Presentation.ViewModels
{
    public class JogoViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [DisplayName("Rodada")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid RodadaId { get; set; }

        [DisplayName("Data")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public DateTime DataJogo { get; set; } = DateTime.Today;
        
        [DisplayName("Horário")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan HoraJogo { get; set; }        
       
        [DisplayName("Estádio")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid EstadioId { get; set; }
        
        [DisplayName("Mandante")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid EquipeCasaId { get; set; }

        [DisplayName("Visitante")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        //[NotEqualTo("EquipeCasaId", ErrorMessage = "Equipe da CASA não pode ser a mesma da equipe VISITANTE !! "),]        
        public Guid EquipeVisitanteId { get; set; }
       
        public int? PlacarCasa { get; set; }

        public int? PlacarVisita { get; set; }

        [DisplayName("Status do Jogo")]
        public StatusJogo Status { get; set; }

        //EF RELATIONS
        public RodadaViewModel Rodada { get; set; }
        public EstadioViewModel Estadio { get; set; }        
        public EquipeCampeonatoViewModel EquipeCasa { get; set; }
        public EquipeCampeonatoViewModel EquipeVisitante { get; set; }

        public IEnumerable<EstadioViewModel> Estadios { get; set; }
        public IEnumerable<EquipeCampeonatoViewModel> EquipesCampeonato { get; set; }


    }
}