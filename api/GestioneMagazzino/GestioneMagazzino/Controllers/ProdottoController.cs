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
    public class ProdottoController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProdottoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize]
        public JsonResult Get()
        {
            string query = @"SELECT * FROM dbo.prodotto";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpGet]
        [Route("InEsaurimento")]
        [Authorize]
        public JsonResult GetProdottiEsaurimento()
        {
            string query = @"SELECT * FROM dbo.prodotto WHERE quantita <= 10";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpGet("{id}")]
        [Authorize]
        public JsonResult Get(int id)
        {
            string query = @"SELECT * FROM dbo.prodotto WHERE idprodotto=@IdProdotto";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@IdProdotto", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpPost]
        [Authorize(Roles = "AddettoForniture")]
        public JsonResult Post(List<Prodotto> prodotto)
        {
            foreach (var prod in prodotto)
            {
                string query = @"INSERT INTO dbo.prodotto 
                                 VALUES(@NOME, @DESCRIZIONE, @PREZZO, @QUANTITA)";

                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@NOME", prod.Nome);
                        myCommand.Parameters.AddWithValue("@DESCRIZIONE", prod.Descrizione);
                        myCommand.Parameters.AddWithValue("@PREZZO", prod.Prezzo);
                        myCommand.Parameters.AddWithValue("@QUANTITA", prod.Quantita);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }
            }
            return new JsonResult("Prodotto aggiunto");
        }

        [HttpPut]
        [Route("SetProdotto")]
        [Authorize(Roles = "Amministratore,AddettoForniture")]
        public JsonResult Put(Prodotto prod)
        {
            string query = @"UPDATE dbo.prodotto 
                             SET nome=@NOME,
                                 descrizione=@DESCRIZIONE, 
                                 prezzo=@PREZZO, 
                                 quantita=@QUANTITA
                             WHERE idprodotto=@IDPRODOTTO";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@IDPRODOTTO", prod.IdProdotto);
                    myCommand.Parameters.AddWithValue("@NOME", prod.Nome);
                    myCommand.Parameters.AddWithValue("@DESCRIZIONE", prod.Descrizione);
                    myCommand.Parameters.AddWithValue("@PREZZO", prod.Prezzo);
                    myCommand.Parameters.AddWithValue("@QUANTITA", prod.Quantita);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Prodotto modificato");
        }

        [HttpPut]
        [Route("SetQuantita")]
        [Authorize(Roles = "Amministratore,AddettoForniture")]
        public JsonResult SetQuantita(Prodotto prod)
        {
            string query = @"UPDATE dbo.prodotto 
                             SET quantita=@QUANTITA
                             WHERE idprodotto=@IDPRODOTTO";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@IDPRODOTTO", prod.IdProdotto);
                    myCommand.Parameters.AddWithValue("@QUANTITA", prod.Quantita);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Prodotto modificato");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Amministratore,AddettoForniture")]
        public JsonResult Delete(int id)
        {
            string query = @"DELETE FROM dbo.prodotto 
                             WHERE idprodotto=@IDPRODOTTO";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@IDPRODOTTO", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Prodotto eliminato");
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
                            myCommand1.Parameters.AddWithValue("@Idprodotto", prodotto.IdProdotto);
                            myCommand1.Parameters.AddWithValue("@Quantita", sum);
                            myReader = myCommand.ExecuteReader();
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
                                myReader = myCommand.ExecuteReader();
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
    }
}
