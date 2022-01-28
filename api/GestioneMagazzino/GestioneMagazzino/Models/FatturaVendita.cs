namespace GestioneMagazzino.Models
{
    public class FatturaVendita
    {
        public int Id { get; set; }
        public string? NFattura { get; set; }
        public DateTime DataVendita { get; set; }
        public int Quantita { get; set; }
        public double PrezzoVendita { get; set; }
        public int CodCliente { get; set; }
        public int CodUtente { get; set; }
        public int CodProdotto { get; set; }
        public bool Pagamento { get; set; }
    }
}
