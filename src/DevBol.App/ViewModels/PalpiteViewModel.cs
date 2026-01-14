using DevBol.Domain.Models.Apostas;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Domain.Models.Jogos;
using DevBol.Presentation.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevBol.Presentation.ViewModels
{
    public class PalpiteViewModel
    {
        [Key]
        public Guid Id { get; set; }
        
        [DisplayName("Apostador")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid ApostadorCampeonatoId { get; set; }

        public Guid JogoId { get; set; }
        public Guid ApostaRodadaId { get; set; } // FK para a ApostaRodada

        public int? PlacarApostaCasa { get; set; } // <<-- Nomes corretos das propriedades -->>
        public int? PlacarApostaVisita { get; set; } // <<-- Nomes corretos das propriedades -->>

        public int Pontos { get; set; }

        /* EF Relations */
        public Jogo Jogo { get; set; }
        public ApostaRodada ApostaRodada { get; set; } // Navegação para ApostaRodada

    }
}