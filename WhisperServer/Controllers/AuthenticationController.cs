using DataManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Whisper.DataManager.Models;
using WhisperCrypto.Algorithms;

namespace WhisperServer.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly WhisperContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationController(IConfiguration configuration, WhisperContext whisperContext)
        {
            _context = whisperContext;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] BaseCredential userCredential)
        {
            var user = _context.User.FirstOrDefault(e => e.Username == userCredential.Username);

            if (user == null)
                return NotFound();

            if (!PBKDF2.Verify(userCredential.Password, user.Salt, user.Password))
                return Unauthorized();

            return Ok(GenerateToken(user));
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterCredential user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || 
                string.IsNullOrWhiteSpace(user.Password) || 
                string.IsNullOrWhiteSpace(user.Email)) return BadRequest();

            if (_context.User.Any(e => e.Username == user.Username || e.Email == user.Email))
                return BadRequest();

            var pass = PBKDF2.HashNewPassword(user.Password);

            var newUser = new User()
            {
                Username = user.Username,
                Email = user.Email,
                Password = pass.Item1,
                Salt = pass.Item2,
                PubKey = user.PubKey,
            };

            _context.User.Add(newUser);
            
            _context.SaveChanges();

            return Ok(GenerateToken(newUser));
        }

        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
