namespace GestioneMagazzino.Models
{
    public class FatturaAcquistoResult
    {
        public int Id { get; set; }
        public string NFattura { get; set; }
        public DateTime DataAcquisto { get; set; }
        public int QuantitaTotale { get; set; }
        public double Saldo { get; set; }
        public int CodFornitore { get; set; }
        public int CodUtente { get; set; }
        public int CodProdotto { get; set; }
    }
}
