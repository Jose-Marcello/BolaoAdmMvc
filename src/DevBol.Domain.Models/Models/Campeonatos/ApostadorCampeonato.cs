using DevBol.Domain.Models.Apostadores;
using DevBol.Domain.Models.Apostas;
using DevBol.Domain.Models.RankingRodadas;

//using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBol.Domain.Models.Base;

namespace DevBol.Domain.Models.Campeonatos
{
    //Será uma entity, mas a chave primária não será o Guid Id e sim
    //uma chave composta pelos Id de Equipe+campeonato (ver o mapping)
    public class ApostadorCampeonato : Entity
    {
        //não vai ser mais :chave primária composta : EquipeId,CampeonatoId
        //tem que ter um mecanismo (index) para garantir a unicidade
        //a chave será o Guid Id da entidade de junção

        public Guid CampeonatoId { get; set; }
        //[Index("IUQ_CampeonatoEquipe_CampeonatoId_EquipeId", IsUnique = true)]

        public Guid ApostadorId { get; set; }
                
        public Campeonato Campeonato { get; set; }
        public JogoFinalizadoEvent Apostador { get; set; }

        public int Pontuacao { get; set; }
        public int Posicao { get; set; }
        //Novas propriedades(campos NOT NULL na tabela)
        public DateTime DataInscricao { get; set; } // Data em que o apostador se inscreveu/aderiu ao campeonato
        public bool CustoAdesaoPago { get; set; } // Indica se o custo de adesão ao campeonato foi pago (true/false)


        //public IEnumerable<Apostador> Apostadores { get; set; }

        public ICollection<RankingRodada> RankingRodadas { get; set; } // Relação para o ranking por rodada
        public ICollection<ApostaRodada> ApostasRodada { get; set; } // <<-- ADICIONADO: Coleção de ApostasRodada

    }
}
