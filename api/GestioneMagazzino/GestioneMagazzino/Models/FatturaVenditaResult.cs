namespace GestioneMagazzino.Models
{
    public class FatturaVenditaResult
    {
        public int Id { get; set; }
        public string NFattura { get; set; }
        public DateTime DataVendita { get; set; }
        public int QuantitaTotale { get; set; }
        public double Saldo { get; set; }
        public int CodCliente { get; set; }
        public int CodUtente { get; set; }
        public int CodProdotto { get; set; }
        public bool Pagamento { get; set; }
    }
}
