using System.ComponentModel.DataAnnotations;

namespace GestioneMagazzino.Models
{
    public class Prodotto
    {
        public int IdProdotto { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public string Descrizione { get; set; }
        [Required]
        public double Prezzo { get; set; }
        [Required]
        public int Quantita { get; set; }
    }
}
