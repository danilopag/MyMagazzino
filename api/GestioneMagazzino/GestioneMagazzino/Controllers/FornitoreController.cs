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
    public class FornitoreController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public FornitoreController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize(Roles = "Amministratore,AddettoForniture")]
        public JsonResult Get()
        {
            string query = @"SELECT * FROM dbo.fornitore";

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
        [Authorize(Roles = "Amministratore,AddettoForniture")]
        public JsonResult Get(int id)
        {
            string query = @"SELECT * FROM dbo.fornitore WHERE idfornitore=@IDFORNITORE";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@IDFORNITORE", id);
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
        public JsonResult Post(List<Fornitore> fornitori)
        {
            foreach (var f in fornitori)
            {
                string query = @"INSERT INTO dbo.fornitore
                             VALUES(@NOME, @PARTITAIVA, @INDIRIZZO, @EMAIL)";

                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@NOME", f.Nome);
                        myCommand.Parameters.AddWithValue("@PARTITAIVA", f.PartitaIva);
                        myCommand.Parameters.AddWithValue("@INDIRIZZO", f.Indirizzo);
                        myCommand.Parameters.AddWithValue("@EMAIL", f.Email);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }
            }
            return new JsonResult("Fornitore creato");
        }

        [HttpPut]
        [Authorize(Roles = "AddettoForniture")]
        public JsonResult Put(Fornitore f)
        {
            string query = @"UPDATE dbo.fornitore 
                             SET nome=@NOME,
                                 partitaiva=@PARTITAIVA, 
                                 indirizzo=@INDIRIZZO, 
                                 email=@EMAIL
                             WHERE idfornitore=@IDFORNITORE";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@IDFORNITORE", f.IdFornitore);
                    myCommand.Parameters.AddWithValue("@NOME", f.Nome);
                    myCommand.Parameters.AddWithValue("@PARTITAIVA", f.PartitaIva);
                    myCommand.Parameters.AddWithValue("@INDIRIZZO", f.Indirizzo);
                    myCommand.Parameters.AddWithValue("@EMAIL", f.Email);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Fornitore modificato");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "AddettoForniture")]
        public JsonResult Delete(int id)
        {
            string query = @"DELETE FROM dbo.fornitore 
                             WHERE idfornitore=@IDFORNITORE";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@IDFORNITORE", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Fornitore eliminato");
        }
    }
}
