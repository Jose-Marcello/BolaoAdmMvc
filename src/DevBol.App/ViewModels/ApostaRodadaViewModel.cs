using DevBol.Presentation.Extensions;
using DevBol.Domain.Models.Apostas;
using DevBol.Domain.Models.Campeonatos;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevBol.Presentation.ViewModels
{
    public class ApostaRodadaViewModel
    {
        [Key]
        public Guid Id { get; set; }
        
        [DisplayName("Apostador")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid ApostadorCampeonatoId { get; set; }

        [DisplayName("Data/Hora Envio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        //[Required(ErrorMessage = "O campo {0} é obrigatório")]        
        public DateTime DataHoraSubmissao { get; set; } //= DateTime.Now; //inicialmente nula

                [DisplayName("Data/Hora Envio")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        //[Required(ErrorMessage = "O campo {0} é obrigatório")]        
        public DateTime DataCriacao { get; set; } //= DateTime.Now; //inicialmente nula
        public bool EhApostaCampeonato { get; set; }    // Indica se esta submissão de palpites conta para o Campeonato (true/false)
        public bool EhApostaIsolada { get; set; }        // Indica se esta submissão de palpites é uma aposta avulsa/isolada (true/false)

        public bool Enviada { get; set; } = false;

        public int PontuacaoTotalRodada { get; set; }

        public decimal? CustoPagoApostaRodada { get; set; }

        //[ForeignKey("CampeonatoId")]
        //public JogoViewModel Jogo { get; set; }

        //alterar para Apostador do Campeonato (para não trazer confusão)
        public ApostadorCampeonatoViewModel ApostadorCampeonato { get; set; }

        public IEnumerable<Palpite> Palpites { get; set; }

    }
}