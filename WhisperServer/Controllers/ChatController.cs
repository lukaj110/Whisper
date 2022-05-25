using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisper.DataManager.Models;

namespace WhisperServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly WhisperContext _context;
        public ChatController(WhisperContext whisperContext) => _context = whisperContext;

        [HttpGet("chat")]
        [Authorize]
        public IActionResult GetActiveChats()
        {
            var currentUser = _context.FromUsername(User.Identity.Name);

            var sentToUsers = _context.Message.Where(e => e.Sender == currentUser.UserId)
                                              .Select(e => e.ChannelId).Distinct();

            var receivedFromUsers = _context.Message
                                            .Include(e => e.SenderNavigation)
                                            .Where(e => e.ChannelId == currentUser.ChannelId)
                                            .Select(e => e.SenderNavigation.ChannelId).Distinct();


            return Ok(_context.User.Where(e => sentToUsers.Contains(e.ChannelId) || receivedFromUsers.Contains(e.ChannelId)));
        }

        [HttpGet("groupchat")]
        [Authorize]
        public IActionResult GetActiveGroups() => Ok(_context.FromUsername(User.Identity.Name).Group);
    }
}
