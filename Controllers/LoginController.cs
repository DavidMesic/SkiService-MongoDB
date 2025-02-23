using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SkiServiceAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MongoDB.Driver;
using SkiServiceAPI.Services;

namespace SkiServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IMongoCollection<Account> _accounts;
        private readonly IConfiguration _config;

        public LoginController(ILogger<LoginController> logger, MongoDbService mongoDbService, IConfiguration config)
        {
            _logger = logger;

            _accounts = mongoDbService.Database.GetCollection<Account>("account");

            _config = config;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] Login accountLogin)
        {
            var user = Authenticate(accountLogin);

            if (user != null)
            {
                var token = Generate(user);  // Generiere das JWT

                // Gebe das Token und die AccountID in der Antwort zurück
                return Ok(new { jwt = token, accountId = user.AccountID });
            }

            return NotFound(new { message = "Account nicht gefunden" });
        }

        private string Generate(Account user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("AccountID", user.AccountID.ToString())  // AccountID als Claim hinzufügen
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private Account Authenticate(Login login)
        {
            var checkUser = _accounts.Find(a => a.Email == login.Email).FirstOrDefault();

            if (checkUser != null)
            {
                bool checkPw = BCrypt.Net.BCrypt.Verify(login.Password, checkUser.Password);
                if (checkPw)
                {
                    return checkUser;
                }
            }

            return null;
        }
    }
}
