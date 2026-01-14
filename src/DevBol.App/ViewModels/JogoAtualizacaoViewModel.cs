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
    public class JogoAtualizacaoViewModel
    {
        public Guid Id { get; set; }

        [DisplayName("Rodada")]
        public Guid RodadaId { get; set; }

        [DisplayName("Estádio")]
        public Guid EstadioId { get; set; }

        [DisplayName("Mandante")]
        public Guid EquipeCasaId { get; set; }

        [DisplayName("Visitante")]
        public Guid EquipeVisitanteId { get; set; }

        public int? PlacarCasa { get; set; }
        public int? PlacarVisita { get; set; }

        [DisplayName("Status do Jogo")]
        public StatusJogo Status { get; set; }
    }
}


