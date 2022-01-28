using GestioneMagazzino.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace GestioneMagazzino.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FatturaVenditaController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public FatturaVenditaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize(Roles = "Amministratore,AddettoVendite")]
        public JsonResult Get()
        {
            List<FatturaVenditaResult> fatturevendita = new List<FatturaVenditaResult>();
            List<FatturaVendita> fattura = new List<FatturaVendita>(); //lista d'appoggio per prelevare i numeri di fattura nel db
            double spesafattura = 0; //variabile d'appoggio per calcolare la spesa tot. di ogni fattura
            int quantitatot = 0; //variabile d'appoggio per calcolare quantita totale
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                try
                {
                    string query = @"SELECT nfattura FROM dbo.fatturavendita GROUP BY nfattura";
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        while (myReader.Read())
                        {
                            FatturaVendita fattura1 = new FatturaVendita(); //Classe di appoggio
                            fattura1.NFattura = myReader.GetString(0);
                            fattura.Add(fattura1);
                        }
                        myReader.Close();
                        foreach (var ft in fattura)
                        {
                            //Azzeramento var di appoggio
                            quantitatot = 0;
                            spesafattura = 0;
                            FatturaVenditaResult fatturavendita = new FatturaVenditaResult();
                            query = "SELECT * " +
                                            "FROM dbo.fatturavendita INNER JOIN dbo.prodotto " +
                                            "ON dbo.fatturavendita.codprodotto=dbo.prodotto.idprodotto " +
                                            "WHERE nfattura=@NumFattura";
                            using (SqlCommand myCommand1 = new SqlCommand(query, myCon))
                            {
                                myCommand1.Parameters.AddWithValue("@NumFattura", ft.NFattura);
                                myReader = myCommand1.ExecuteReader();
                                while (myReader.Read())
                                {
                                    fatturavendita.NFattura = myReader["nfattura"].ToString();
                                    spesafattura = spesafattura + (Convert.ToInt32(myReader["quantita"]) * Convert.ToDouble(myReader["prezzovendita"]));
                                    quantitatot = quantitatot + Convert.ToInt32(myReader["quantita"]);
                                    fatturavendita.DataVendita = Convert.ToDateTime(myReader["datavendita"]);
                                    fatturavendita.Pagamento = Convert.ToBoolean(myReader["pagamento"]);
                                    fatturavendita.CodCliente = Convert.ToInt32(myReader["codcliente"]);
                                    fatturavendita.CodUtente = Convert.ToInt32(myReader["codutente"]);
                                    //fatturavendita.CodProdotto = Convert.ToInt32(myReader["codprodotto"]);
                                }
                            }
                            fatturavendita.Saldo = spesafattura;
                            fatturavendita.QuantitaTotale = quantitatot;
                            myReader.Close();
                            fatturevendita.Add(fatturavendita);
                        }
                        myCon.Close();
                        return new JsonResult(fatturevendita);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return new JsonResult("Errore!");
                }
            }
            return new JsonResult(fatturevendita);
        }

        [HttpGet("{num}")]
        [Authorize(Roles = "Amministratore,AddettoVendite")]
        public JsonResult Get(int num)
        {
            string query = @"SELECT * FROM dbo.fatturavendita WHERE nfattura=@NumFattura";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                try
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@NumFattura", num);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return new JsonResult("Errore!");
                }
            }

            return new JsonResult(table);
        }

        [HttpGet]
        [Route("VenditeGiornaliere")]
        [Authorize(Roles = "Amministratore,AddettoVendite")]
        public JsonResult VenditeGiornaliere()
        {
            double totaleVendite = 0;

            string query = @"SELECT quantita, prezzovendita
                             FROM dbo.fatturavendita
                             WHERE convert(varchar(10), datavendita, 102) = convert(varchar(10), current_timestamp, 102)";

            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                try
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        while (myReader.Read())
                        {
                            totaleVendite += (Convert.ToInt32(myReader["quantita"]) * Convert.ToDouble(myReader["prezzovendita"]));
                        }
                        myReader.Close();
                        myCon.Close();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return new JsonResult("Errore!");
                }
            }

            return new JsonResult(totaleVendite);
        }

        [HttpPost]
        [Authorize(Roles = "AddettoVendite")]
        public JsonResult Post(List<FatturaVendita> fatture)
        {
            var nfattura = GetNumeroFattura();
            if (nfattura != "errore")
            {
                var tmp = Convert.ToInt32(nfattura) + 1;
                nfattura = tmp.ToString();
                foreach (var fattura in fatture)
                {
                    string query = @"INSERT INTO dbo.fatturavendita
                             VALUES(@NFattura,CURRENT_TIMESTAMP, @Quantita,@PrezzoVendita, @IdCliente, @IdProdotto, @IdUtente, @Pagamento)";

                    DataTable table = new DataTable();
                    string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
                    SqlDataReader myReader;
                    using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                    {
                        try
                        {
                            myCon.Open();
                            using (SqlCommand myCommand = new SqlCommand(query, myCon))
                            {
                                myCommand.Parameters.AddWithValue("@NFattura", nfattura);
                                myCommand.Parameters.AddWithValue("@Quantita", fattura.Quantita);
                                myCommand.Parameters.AddWithValue("@PrezzoVendita", fattura.PrezzoVendita);
                                myCommand.Parameters.AddWithValue("@IdCliente", fattura.CodCliente);
                                myCommand.Parameters.AddWithValue("@IdProdotto", fattura.CodProdotto);
                                myCommand.Parameters.AddWithValue("@IdUtente", fattura.CodUtente);
                                myCommand.Parameters.AddWithValue("@Pagamento", fattura.Pagamento ? 1 : 0);
                                if (Minusquantitaprodotto(fattura.CodProdotto, fattura.Quantita))
                                {
                                    myReader = myCommand.ExecuteReader();
                                    myReader.Close();
                                    myCon.Close();
                                }
                                else
                                {
                                    myCon.Close();
                                    return new JsonResult("Errore nella creazione della fattura");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            return new JsonResult("Errore!");
                        }
                    }
                }
            }
            else
            {
                return new JsonResult("Errore!");
            }
            return new JsonResult("Fattura creata");
        }

        [HttpPut("{nfattura}")]
        [Authorize(Roles = "AddettoVendite")]
        public JsonResult AlterPagamento(string nfattura)
        {
            string query = @"UPDATE dbo.fatturavendita
                             SET pagamento=@Pagamento
                             WHERE nfattura LIKE @NumFattura";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                try
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@NumFattura", nfattura);
                        myCommand.Parameters.AddWithValue("@Pagamento", 1);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return new JsonResult("Errore!");
                }
            }

            return new JsonResult("Pagamento modificato");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "AddettoVendite")]
        public JsonResult Delete(string id)
        {
            string query = @"DELETE FROM dbo.fatturavendita
                             WHERE nfattura LIKE @NumFattura";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                try
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@NumFattura", id);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return new JsonResult("Errore!");
                }
            }

            return new JsonResult("Fattura eliminata");
        }

        [Authorize(Roles = "Amministratore,AddettoVendite")]
        public bool Minusquantitaprodotto(int Idprodotto, int Quantita)
        {
            Prodotto prodotto = new Prodotto();
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            try
            {
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    string query = "SELECT * FROM dbo.prodotto WHERE dbo.prodotto.idprodotto=@IdProdotto";
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@IdProdotto", Idprodotto);
                        myReader = myCommand.ExecuteReader();
                        while (myReader.Read())
                        {
                            prodotto.IdProdotto = Convert.ToInt32(myReader["idprodotto"]);
                            prodotto.Nome = myReader["nome"].ToString();
                            prodotto.Descrizione = myReader["descrizione"].ToString();
                            prodotto.Prezzo = Convert.ToDouble(myReader["prezzo"]);
                            prodotto.Quantita = Convert.ToInt32(myReader["quantita"]);
                        }
                        myReader.Close();
                        query = "UPDATE dbo.prodotto SET dbo.prodotto.quantita = @Quantita WHERE dbo.prodotto.idprodotto = @Idprodotto";
                        using (SqlCommand myCommand1 = new SqlCommand(query, myCon))
                        {
                            int minus = prodotto.Quantita - Quantita;
                            myCommand1.Parameters.AddWithValue("@Idprodotto", prodotto.IdProdotto);
                            myCommand1.Parameters.AddWithValue("@Quantita", minus);
                            if (minus >= 0)
                            {
                                myReader = myCommand1.ExecuteReader();
                                table.Load(myReader);
                                myReader.Close();
                            }
                            else
                            {
                                myReader.Close();
                                myCon.Close();
                                return false;
                            }
                        }
                    }
                    myCon.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        [Authorize(Roles = "Amministratore,AddettoForniture")]

        public string GetNumeroFattura() //Recupera l'ultimo numero fattura
        {
            string query = @"SELECT nfattura FROM dbo.fatturavendita WHERE datavendita=(
                             SELECT MAX(datavendita) FROM dbo.fatturavendita)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                try
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        if (myReader.HasRows)
                        {
                            while (myReader.Read())
                            {
                                var nfattura = myReader["nfattura"].ToString();
                                myReader.Close();
                                myCon.Close();
                                return nfattura;
                            }
                        }
                        else
                        {
                            myReader.Close();
                            myCon.Close();
                            return "1";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return "errore";
                }
            }
            return "errore";
        }
    }
}
