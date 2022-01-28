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
    public class ClienteController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ClienteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize(Roles = "Amministratore,AddettoVendite")]
        public JsonResult Get()
        {
            string query = @"SELECT * FROM dbo.cliente";

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
        [Authorize(Roles = "Amministratore,AddettoVendite")]
        public JsonResult Get(int id)
        {
            string query = @"SELECT * FROM dbo.cliente WHERE idcliente=@IDCLIENTE";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@IDCLIENTE", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpPost]
        [Authorize(Roles = "AddettoVendite")]
        public JsonResult Post(List<Cliente> clienti)
        {
            foreach (var c in clienti)
            {
                string query = @"INSERT INTO dbo.cliente
                             VALUES(@NOME, @PARTITAIVA, @CODICEFISCALE, @INDIRIZZO, @EMAIL)";

                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@NOME", c.Nome);
                        myCommand.Parameters.AddWithValue("@PARTITAIVA", c.PartitaIva);
                        myCommand.Parameters.AddWithValue("@CODICEFISCALE", c.CodiceFiscale);
                        myCommand.Parameters.AddWithValue("@INDIRIZZO", c.Indirizzo);
                        myCommand.Parameters.AddWithValue("@EMAIL", c.Email);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }
            }
            return new JsonResult("Cliente creato");
        }

        [HttpPut]
        [Authorize(Roles = "AddettoVendite")]
        public JsonResult Put(Cliente c)
        {
            string query = @"UPDATE dbo.cliente 
                             SET nome=@NOME,
                                 partitaiva=@PARTITAIVA,
                                 codicefiscale=@CODICEFISCALE, 
                                 indirizzo=@INDIRIZZO, 
                                 email=@EMAIL
                             WHERE idcliente=@IDCLIENTE";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@IDCLIENTE", c.IdCliente);
                    myCommand.Parameters.AddWithValue("@NOME", c.Nome);
                    myCommand.Parameters.AddWithValue("@PARTITAIVA", c.PartitaIva);
                    myCommand.Parameters.AddWithValue("@CODICEFISCALE", c.CodiceFiscale);
                    myCommand.Parameters.AddWithValue("@INDIRIZZO", c.Indirizzo);
                    myCommand.Parameters.AddWithValue("@EMAIL", c.Email);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Cliente modificato");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "AddettoVendite")]
        public JsonResult Delete(int id)
        {
            string query = @"DELETE FROM dbo.cliente
                             WHERE idcliente=@IDCLIENTE";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@IDCLIENTE", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Cliente eliminato");
        }
    }
}
