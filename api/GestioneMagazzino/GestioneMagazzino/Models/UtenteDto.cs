using System.ComponentModel.DataAnnotations;

namespace GestioneMagazzino.Models
{
    public class UtenteDto
    {
        public string? IdUtente { get; set; }
        [Required]
        public string? Username { get; set; }
        public string? Nome { get; set; }
        public string? Cognome { get; set; }
        public string? Password { get; set; }
        [Required]
        public RoleType IsAdmin { get; set; }
    }
}
