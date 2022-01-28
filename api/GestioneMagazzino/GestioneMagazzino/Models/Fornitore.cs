using System.ComponentModel.DataAnnotations;

namespace GestioneMagazzino.Models
{
    public class Fornitore
    {
        public int IdFornitore { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public string PartitaIva { get; set; }
        [Required]
        public string Indirizzo { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
