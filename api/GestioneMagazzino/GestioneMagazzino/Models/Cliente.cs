using System.ComponentModel.DataAnnotations;

namespace GestioneMagazzino.Models
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public string PartitaIva { get; set; }
        [Required]
        public string CodiceFiscale { get; set; }
        [Required]
        public string Indirizzo { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
