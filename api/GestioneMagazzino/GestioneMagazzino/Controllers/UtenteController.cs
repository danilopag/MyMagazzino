using GestioneMagazzino.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace GestioneMagazzino.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtenteController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UtenteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize]
        public JsonResult Get()
        {
            string query = @"SELECT * FROM dbo.utente";

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
            string query = @"SELECT * FROM dbo.utente WHERE idutente=@IDUTENTE";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@IdUtente", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpPost]
        [Route("Register")]
        [Authorize(Roles = "Amministratore")]
        public async Task<ActionResult<Utente>> Register(UtenteDto request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            string query = @"INSERT INTO dbo.utente 
                             VALUES(@USERNAME, @NOME, @COGNOME, @PASSWORD, @PASSWORDSALT, @ISADMIN)";

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
                        myCommand.Parameters.AddWithValue("@USERNAME", request.Username);
                        myCommand.Parameters.AddWithValue("@NOME", request.Nome);
                        myCommand.Parameters.AddWithValue("@COGNOME", request.Cognome);
                        myCommand.Parameters.AddWithValue("@PASSWORD", passwordHash);
                        myCommand.Parameters.AddWithValue("@PASSWORDSALT", passwordSalt);
                        myCommand.Parameters.AddWithValue("@ISADMIN", (int)request.IsAdmin);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }
                catch (SqlException ex)
                {
                    StringBuilder errorMessages = new StringBuilder();
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append("Index #" + i + "\n" +
                            "Message: " + ex.Errors[i].Message + "\n" +
                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                            "Source: " + ex.Errors[i].Source + "\n" +
                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    Console.WriteLine(errorMessages.ToString());
                    return BadRequest("Registrazione fallita!");
                }
            }
            return Ok("Utente registrato correttamente");
        }

        [HttpPut]
        [Authorize(Roles = "Amministratore")]
        public JsonResult Put(UtenteDto u)
        {
            string query = @"UPDATE dbo.utente 
                             SET username=@USERNAME,
                                 nome=@NOME,
                                 cognome=@COGNOME, 
                                 isadmin=@ISADMIN
                             WHERE idutente=@IDUTENTE";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@IDUTENTE", u.IdUtente);
                    myCommand.Parameters.AddWithValue("@USERNAME", u.Username);
                    myCommand.Parameters.AddWithValue("@NOME", u.Nome);
                    myCommand.Parameters.AddWithValue("@COGNOME", u.Cognome);
                    myCommand.Parameters.AddWithValue("@ISADMIN", u.IsAdmin);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Utente modificato");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Amministratore")]
        public JsonResult Delete(int id)
        {
            string query = @"DELETE FROM dbo.utente 
                             WHERE idutente=@IDUTENTE";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@IDUTENTE", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Utente eliminato");
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
