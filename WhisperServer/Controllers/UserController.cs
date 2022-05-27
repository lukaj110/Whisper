using Whisper.DataManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Whisper.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly WhisperContext _context;

        public UserController(WhisperContext whisperContext) => _context = whisperContext;

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers() => Ok(_context.User.AsEnumerable());

        [HttpGet("me")]
        [Authorize]
        public ActionResult<User> Me()
        {
            return Ok(_context.FromIdentity(HttpContext.User));
        }

        [HttpGet("by-id/{userId}")]
        [Authorize]
        public ActionResult<User> GetUser(int userId)
        {
            var user = _context.User.FirstOrDefault(e => e.UserId == userId);

            if (user == null) return NotFound();

            return Ok(new { user.UserId, user.Email, user.Username, user.PubKey, user.ChannelId });
        }

        [HttpGet("by-username/{username}")]
        [Authorize]
        public ActionResult<User> GetUserByUsername(string username)
        {
            var user = _context.User.FirstOrDefault(e => e.Username == username);

            if (user == null) return NotFound();

            return Ok(new { user.UserId, user.Email, user.Username, user.PubKey, user.ChannelId });
        }

        [HttpGet("by-email/{email}")]
        [Authorize]
        public ActionResult<User> GetUserByEmail(string email)
        {
            var user = _context.User.FirstOrDefault(e => e.Email == email);

            if (user == null) return NotFound();

            return Ok(new { user.UserId, user.Email, user.Username, user.PubKey, user.ChannelId });
        }
    }
}
