using DevBol.Presentation.Extensions;
using DevBol.Domain.Models.Apostas;
using DevBol.Domain.Models.Campeonatos;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevBol.Presentation.ViewModels
{
    public class SalvarApostaViewModel
    {
        [Key]
        public Guid Id { get; set; }        
        
        [Range(0,20)]        
        public int PlacarApostaCasa  { get; set; }
        
        [Range(0, 20)]
        public int PlacarApostaVisita { get; set; }      

       
    }
}