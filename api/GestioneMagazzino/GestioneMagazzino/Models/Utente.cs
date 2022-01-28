using System.ComponentModel.DataAnnotations;

namespace GestioneMagazzino.Models
{
    public class Utente
    {
        public string? IdUtente { get; set; }
        [Required]
        public string? Username { get; set; }
        public string? Nome { get; set; }
        public string? Cognome { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        [Required]
        public RoleType IsAdmin { get; set; }
    }
}
