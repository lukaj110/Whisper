using Whisper.DataManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;

namespace Whisper.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly WhisperContext _context;

        public GroupController() => _context = new WhisperContext();

        [HttpGet]
        public ActionResult<IEnumerable<Group>> GetUsers()
        {
            return Ok(_context.Group.Include(e => e.User).ToList());
        }
    }
}
