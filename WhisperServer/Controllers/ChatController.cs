using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisper.DataManager.Models;

namespace Whisper.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly WhisperContext _context;
        public ChatController(WhisperContext whisperContext) => _context = whisperContext;

        [HttpGet("active")]
        [Authorize]
        public ActionResult GetActiveChats()
        {
            var currentUser = _context.FromIdentity(HttpContext.User);

            var sentToUsers = _context.Message.Where(e => e.Sender == currentUser.UserId)
                                              .Select(e => e.ChannelId).Distinct();

            var receivedFromUsers = _context.Message
                                            .Include(e => e.SenderNavigation)
                                            .Where(e => e.ChannelId == currentUser.ChannelId)
                                            .Select(e => e.SenderNavigation.ChannelId).Distinct();


            return Ok(_context.User.Where(e => sentToUsers.Contains(e.ChannelId) || receivedFromUsers.Contains(e.ChannelId))
                .Select(user => new { user.UserId, user.Email, user.Username, user.PubKey, user.ChannelId }));
        }

        [HttpGet("activegroups")]
        [Authorize]
        public ActionResult GetActiveGroups() => Ok(_context.FromIdentity(HttpContext.User).Group);
    }
}
