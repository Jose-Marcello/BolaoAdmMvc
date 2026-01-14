using DevBol.Domain.Models.Apostas;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Domain.Models.Estadios;
using DevBol.Domain.Models.Rodadas;
using DevBol.Domain.Models.Base;

namespace DevBol.Domain.Models.Jogos

{     
    public class Jogo : Entity
    {

        public Guid RodadaId { get; set; }

        public DateTime DataJogo { get; set; }
        public TimeSpan HoraJogo { get; set; }

        public Guid EstadioId { get; set; }
        
        public Guid EquipeCasaId { get; set; }
        
        public Guid EquipeVisitanteId { get; set; }

        public int? PlacarCasa { get; set; } = null;
        public int? PlacarVisita { get; set; } = null;

        public StatusJogo Status { get; set; }

        //EF RELATIONS  

        public Rodada Rodada { get; set; }
        public Estadio Estadio { get; set; }

        public EquipeCampeonato EquipeCasa { get; set; }       
        public EquipeCampeonato EquipeVisitante { get; set; }

        public ICollection<Palpite> Palpites { get; set; }
    }
}