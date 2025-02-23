using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SkiServiceAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SkiServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private readonly SkiServiceContext _context;

        private readonly IConfiguration _config;

        public LoginController(ILogger<LoginController> logger, SkiServiceContext context, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _config = config;
        }



        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] AccountLogin accountLogin)
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
        new Claim(ClaimTypes.NameIdentifier, user.Benutzername),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Rolle),
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



        private Account Authenticate(AccountLogin accountLogin)
        {
            var checkUser = _context.Accounts.FirstOrDefault(a => a.Email == accountLogin.Email);

            if (checkUser != null)
            {
                bool checkPw = BCrypt.Net.BCrypt.Verify(accountLogin.PasswortHash, checkUser.PasswortHash);

                if (checkPw)
                {
                    return checkUser;
                }
            }

            return null;
        }
    }
}
