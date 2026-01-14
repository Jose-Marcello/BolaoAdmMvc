using DevBol.Domain.Models.Ufs;
using DevBol.Domain.Models.Base;


namespace DevBol.Domain.Models.Estadios

{     
    public class Estadio : Entity
    {       
        public string Nome { get; set; }
        
        public Guid UfId { get; set; }

        /* EF Relations */
        public Uf Uf { get; set; }           

    }
}