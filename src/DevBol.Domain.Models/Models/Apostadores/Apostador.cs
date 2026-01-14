using DevBol.Domain.Models.Base;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Domain.Models.Usuarios;

namespace DevBol.Domain.Models.Apostadores
{  
        public class JogoFinalizadoEvent : Entity
        {
            public StatusApostador Status { get; set; }
            public string UsuarioId { get; set; }
            public Usuario Usuario { get; set; }

            public IEnumerable<ApostadorCampeonato> ApostadoresCampeonatos { get; set; }


        }
    
}