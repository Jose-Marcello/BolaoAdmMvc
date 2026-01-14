using DevBol.Domain.Models.Base;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Domain.Models.Rodadas;


namespace DevBol.Domain.Models.RankingRodadas
{
    public class RankingRodada : Entity
    {
        public Guid RodadaId { get; set; }
        public Guid ApostadorCampeonatoId { get; set; }
        public int Pontuacao { get; set; }
        public int Posicao { get; set; }
        public DateTime DataAtualizacao { get; set; }

        public Rodada Rodada { get; set; }
        public ApostadorCampeonato ApostadorCampeonato { get; set; }

    }

}
