using DevBol.Domain.Models.Apostas;
using DevBol.Domain.Models.Base;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Domain.Models.Jogos;
using DevBol.Domain.Models.RankingRodadas;

namespace DevBol.Domain.Models.Rodadas{ 
    
    public class Rodada : Entity
    {
        public Guid CampeonatoId { get; set; }

        public int NumeroRodada { get; set; }

        public DateTime DataInic { get; set; }
       
        public DateTime DataFim { get; set; }

        public int NumJogos { get; set; }

        //public bool Ativa { get; set; }

        public StatusRodada Status { get; set; }
        //public bool Status { get; set; }


        /* EF Relations */
        public Campeonato Campeonato { get; set; }
       
        public IEnumerable<Jogo> JogosRodada { get; set; }

        public IEnumerable<RankingRodada> RankingRodadas { get; set; }

        public ICollection<ApostaRodada> ApostasRodada { get; set; }


    }
}