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
    public class FatturaAcquistoController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public FatturaAcquistoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize(Roles = "Amministratore,AddettoForniture")]
        public JsonResult Get()
        {
            List<FatturaAcquistoResult> fattureacquisto = new List<FatturaAcquistoResult>();
            List<FatturaAcquisto> fattura = new List<FatturaAcquisto>(); //lista d'appoggio per prelevare i numeri di fattura nel db
            double spesafattura = 0; //variabile d'appoggio per calcolare la spesa tot. di ogni fattura
            int quantitatot = 0; //variabile d'appoggio per calcolare quantita totale
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                try
                {
                    string query = @"SELECT nfattura, codfornitore FROM dbo.fatturaacquisto GROUP BY nfattura, codfornitore";
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        while (myReader.Read())
                        {
                            FatturaAcquisto fattura1 = new FatturaAcquisto(); //Classe di appoggio
                            fattura1.NFattura = myReader["nfattura"].ToString();
                            fattura1.CodFornitore = Convert.ToInt32(myReader["codfornitore"]);
                            fattura.Add(fattura1);
                        }
                        myReader.Close();
                        foreach (var ft in fattura)
                        {
                            //Azzeramento var di appoggio
                            quantitatot = 0;
                            spesafattura = 0;
                            FatturaAcquistoResult fatturaacquisto = new FatturaAcquistoResult();
                            query = "SELECT * " +
                                            "FROM dbo.fatturaacquisto INNER JOIN dbo.prodotto " +
                                            "ON dbo.fatturaacquisto.codprodotto=dbo.prodotto.idprodotto " +
                                            "WHERE dbo.fatturaacquisto.nfattura=@NumFattura AND dbo.fatturaacquisto.codfornitore=@IdFornitore ";
                            using (SqlCommand myCommand1 = new SqlCommand(query, myCon))
                            {
                                myCommand1.Parameters.AddWithValue("@NumFattura", ft.NFattura);
                                myCommand1.Parameters.AddWithValue("@IdFornitore", ft.CodFornitore);
                                myReader = myCommand1.ExecuteReader();
                                while (myReader.Read())
                                {
                                    fatturaacquisto.NFattura = myReader["nfattura"].ToString();
                                    spesafattura = spesafattura + (Convert.ToInt32(myReader["quantita"]) * Convert.ToDouble(myReader["prezzoacquisto"]));
                                    quantitatot = quantitatot + Convert.ToInt32(myReader["quantita"]);
                                    fatturaacquisto.DataAcquisto = Convert.ToDateTime(myReader["dataacquisto"]);
                                    fatturaacquisto.CodFornitore = Convert.ToInt32(myReader["codfornitore"]);
                                    fatturaacquisto.CodUtente = Convert.ToInt32(myReader["codutente"]);
                                    //fatturaacquisto.CodProdotto = Convert.ToInt32(myReader["codprodotto"]);
                                }
                            }
                            fatturaacquisto.Saldo = spesafattura;
                            fatturaacquisto.QuantitaTotale = quantitatot;
                            myReader.Close();
                            fattureacquisto.Add(fatturaacquisto);
                        }
                        myCon.Close();
                        return new JsonResult(fattureacquisto);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return new JsonResult("Errore!");
                }
            }
            return new JsonResult(fattureacquisto);
        }

        [HttpGet]
        [Route("{codfornitore}/{num}")]
        [Authorize(Roles = "Amministratore,AddettoForniture")]
        public JsonResult Get(int codfornitore, int num)
        {
            string query = @"SELECT * FROM dbo.fatturaacquisto WHERE nfattura=@NUMFATTURA AND codfornitore=@CODFORNITORE";

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
                        myCommand.Parameters.AddWithValue("@NUMFATTURA", num);
                        myCommand.Parameters.AddWithValue("@CODFORNITORE", codfornitore);
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
        [Route("AcquistiGiornalieri")]
        [Authorize(Roles = "Amministratore,AddettoForniture")]
        public JsonResult AcquistiGiornalieri()
        {
            double totaleAcquisti = 0;

            string query = @"SELECT quantita, prezzoacquisto
                             FROM dbo.fatturaacquisto
                             WHERE convert(varchar(10), dataacquisto, 102) = convert(varchar(10), current_timestamp, 102)";

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
                            totaleAcquisti += (Convert.ToInt32(myReader["quantita"]) * Convert.ToDouble(myReader["prezzoacquisto"]));
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

            return new JsonResult(totaleAcquisti);
        }

        [HttpPost]
        [Authorize(Roles = "AddettoForniture")]
        public JsonResult Post(List<FatturaAcquisto> fatture)
        {
            foreach (var fattura in fatture)
            {
                string query = @"INSERT INTO dbo.fatturaacquisto
                             VALUES(@NFattura, CURRENT_TIMESTAMP, @Quantita, @PrezzoAcquisto, @IdFornitore, @IdProdotto, @IdUtente)";

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
                            myCommand.Parameters.AddWithValue("@NFattura", fattura.NFattura);
                            myCommand.Parameters.AddWithValue("@Quantita", fattura.Quantita);
                            myCommand.Parameters.AddWithValue("@PrezzoAcquisto", fattura.PrezzoAcquisto);
                            myCommand.Parameters.AddWithValue("@IdFornitore", fattura.CodFornitore);
                            myCommand.Parameters.AddWithValue("@IdProdotto", fattura.CodProdotto);
                            myCommand.Parameters.AddWithValue("@IdUtente", fattura.CodUtente);
                            if (Addquantitaprodotto(fattura.CodProdotto, fattura.Quantita))
                            {
                                myReader = myCommand.ExecuteReader();
                                table.Load(myReader);
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
            return new JsonResult("Fattura creata");
        }

        [HttpPut]
        [Authorize(Roles = "AddettoForniture")]
        public JsonResult Put(FatturaAcquisto fattura)
        {
            string query = @"UPDATE dbo.fatturaacquisto
                             SET dataacquisto=@Data,
                                 quantita=@Quantita,
                                 prezzoacquisto=@PrezzoAcquisto,
                                 codfornitore=@IdFornitore,
                                 codprodotto=@IdProdotto,
                                 codutente=@IdUtente
                             WHERE nfattura=@NumFattura";

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
                        myCommand.Parameters.AddWithValue("@NumFattura", fattura.NFattura);
                        myCommand.Parameters.AddWithValue("@Data", fattura.DataAcquisto);
                        myCommand.Parameters.AddWithValue("@Quantita", fattura.Quantita);
                        myCommand.Parameters.AddWithValue("@PrezzoAcquisto", fattura.PrezzoAcquisto);
                        myCommand.Parameters.AddWithValue("@IdFornitore", fattura.CodFornitore);
                        myCommand.Parameters.AddWithValue("@IdProdotto", fattura.CodProdotto);
                        myCommand.Parameters.AddWithValue("@IdUtente", fattura.CodUtente);
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

            return new JsonResult("Fattura modificata");
        }

        [HttpDelete]
        [Route("{codfornitore}/{num}")]
        [Authorize(Roles = "AddettoForniture")]
        public JsonResult Delete(int codfornitore, int num)
        {
            string query = @"DELETE FROM dbo.fatturaacquisto
                             WHERE nfattura LIKE @NUMFATTURA AND codfornitore=@CODFORNITORE";

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
                        myCommand.Parameters.AddWithValue("@NUMFATTURA", num);
                        myCommand.Parameters.AddWithValue("@CODFORNITORE", codfornitore);
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

        [Authorize(Roles = "Amministratore,AddettoForniture")]
        public bool Addquantitaprodotto(int Idprodotto, int Quantita)
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
                            int sum = Quantita + prodotto.Quantita;
                            Console.WriteLine(sum);
                            Console.WriteLine(prodotto.IdProdotto);
                            myCommand1.Parameters.AddWithValue("@Idprodotto", Idprodotto);
                            myCommand1.Parameters.AddWithValue("@Quantita", sum);
                            myReader = myCommand1.ExecuteReader();
                            table.Load(myReader);
                            myReader.Close();
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
            string query = @"SELECT nfattura FROM dbo.fatturaacquisto WHERE dataacquisto=(
                             SELECT MAX(dataacquisto) FROM dbo.fatturaacquisto";

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
                            var nfattura = myReader.GetString(0);
                            myReader.Close();
                            myCon.Close();
                            return nfattura;
                        }
                        else
                        {
                            myReader.Close();
                            myCon.Close();
                            return "errore";
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
