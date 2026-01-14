using DevBol.Domain.Models.Base;
using DevBol.Domain.Models.Rodadas;

namespace DevBol.Domain.Models.Campeonatos
{
    public class Campeonato : Entity
    {

       
        public string Nome { get; set; }
        
        public DateTime DataInic { get; set; }
       
        public DateTime DataFim { get; set; }
       
        public int NumRodadas { get; set; }
        
        public TiposCampeonato Tipo { get; set; }        
        public bool Ativo { get; set; }

        /* EF Relations */
        public IEnumerable<EquipeCampeonato> EquipesCampeonatos { get; set; }
        public IEnumerable<ApostadorCampeonato> ApostadoresCampeonatos { get; set; }
        public IEnumerable<Rodada> Rodadas { get; set; }

    }
}