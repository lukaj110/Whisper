using Whisper.DataManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Whisper.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly WhisperContext _context;

        public UserController(WhisperContext whisperContext) => _context = whisperContext;

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Ok(_context.User.Include(e => e.OwnerGroup).Include(e => e.Group).ToList());
        }
    }
}
