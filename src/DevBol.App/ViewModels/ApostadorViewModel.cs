using DevBol.Presentation.Extensions;
using DevBol.Domain.Models.Apostadores;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevBol.Domain.Models.Usuarios;
using DevBol.Domain.Models.Campeonatos;

namespace DevBol.Presentation.ViewModels
{
    public class ApostadorViewModel
    {
        [Key]
        public Guid Id { get; set; }
        public StatusApostador Status { get; set; }
        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public IEnumerable<ApostadorCampeonato> ApostadoresCampeonatos { get; set; }


    }
}