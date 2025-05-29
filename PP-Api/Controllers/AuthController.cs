using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PP_Api.Modelos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PP_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UsuarioModel model)
        {
            if(model.UserName == "admin" &&  model.Password == "password") 
            {
                string token = GenerarJwtToken(model.UserName);
                return Ok(new { model, token });
            }

            return Unauthorized();
        }

        private string GenerarJwtToken(string userName)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "localhost/",
                audience: "localhost/",
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"]!)),
                signingCredentials: creds,
                claims: new[] { new Claim(ClaimTypes.Name, userName) }
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
