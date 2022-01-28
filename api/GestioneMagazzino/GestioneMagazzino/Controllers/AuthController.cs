using GestioneMagazzino.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace GestioneMagazzino.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // public static Utente utente = new Utente();
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Login(UtenteDto request)
        {
            //Aggingere Data Annotation
            if (request.Username != null)
            {
                string query = @"SELECT * FROM dbo.utente WHERE username LIKE @USERNAME";
                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("GestioneMagazzinoAppCon");
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@USERNAME", request.Username);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }
                if (table.Rows.Count > 0)
                {
                    if (!VerifyPasswordHash(request.Password, (byte[])table.Rows[0]["password"], (byte[])table.Rows[0]["passwordsalt"]))
                    {
                        return BadRequest("Wrong password.");
                    }
                    request.IsAdmin = (RoleType)table.Rows[0]["isadmin"];
                    request.IdUtente = (table.Rows[0]["idutente"]).ToString();
                    var token = CreateToken(request);
                    return Ok(token);
                }
                else
                {
                    return BadRequest("Errore nel login!");
                }
            }
            return BadRequest("Errore nel login!");
        }

        private string CreateToken(UtenteDto user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdUtente),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.IsAdmin.ToString())
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("JwtConfig:Secret").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var issuer = "http://127.0.0.1:5107/";

            var token = new JwtSecurityToken(
                issuer: issuer,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
