namespace GestioneMagazzino.Models
{
    public class FatturaAcquisto
    {
        public int Id { get; set; }
        public string? NFattura { get; set; }
        public DateTime DataAcquisto { get; set; }
        public int Quantita { get; set; }
        public double PrezzoAcquisto { get; set; }
        public int CodFornitore { get; set; }
        public int CodUtente { get; set; }
        public int CodProdotto { get; set; }
    }
}
