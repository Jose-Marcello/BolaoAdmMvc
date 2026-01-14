using DevBol.Domain.Models.Base;
using DevBol.Domain.Models.Equipes;
using DevBol.Domain.Models.Estadios;

namespace DevBol.Domain.Models.Ufs

{     
    public class Uf : Entity
    {        
        public string Nome { get; set; }

        public string Sigla { get; set; }

        /* EF Relations */
        public IEnumerable<Equipe> Equipes { get; set; }

        public IEnumerable<Estadio> Estadios { get; set; }



    }
}