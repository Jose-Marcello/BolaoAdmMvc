using DevBol.Domain.Models.Apostadores;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DevBol.Domain.Models.Usuarios

{
    public class Usuario : IdentityUser<string>
    {

        [Required]
        [MaxLength(11)]
        public string CPF { get; set; }

        [MaxLength(20)]
        public string Celular { get; set; }

        [MaxLength(50)]
        public string Apelido { get; set; }

        public JogoFinalizadoEvent Apostador { get; set; }

        public DateTime? RegistrationDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }

}