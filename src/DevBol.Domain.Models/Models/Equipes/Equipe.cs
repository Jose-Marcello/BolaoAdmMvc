using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using DevBol.Domain.Models.Equipes;
using DevBol.Domain.Models.Ufs;
using DevBol.Domain.Models.Campeonatos;
using DevBol.Domain.Models.Jogos;
using DevBol.Domain.Models.Base;


namespace DevBol.Domain.Models.Equipes

{     
    public class Equipe : Entity
    {       
        public string Nome { get; set; }
        public string Sigla { get; set; }

        //sair daqui - ir p/ viewModel ?
        [NotMapped]
        [DisplayName("Imagem do Escudo")]
        public IFormFile EscudoUpload { get; set; }
       
        public string Escudo { get; set; }
       
        public TiposEquipe Tipo { get; set; }

        public Guid UfId { get; set; }

        /* EF Relations */
        public Uf Uf { get; set; }
        
        public IEnumerable<EquipeCampeonato> EquipesCampeonatos { get; set; }          



    }
}