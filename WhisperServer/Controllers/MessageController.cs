using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using Whisper.DataManager.Models;
using Whisper.DataManager.RequestModels;

namespace Whisper.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly WhisperContext _context;
        public MessageController(WhisperContext whisperContext, List<WebSocket> webSocketList)
        {
            _context = whisperContext;
        }

        [HttpPost]
        [Authorize]
        public ActionResult SendMessage([FromBody] MessageModel message)
        {
            var user = _context.FromIdentity(HttpContext.User);

            var newMessage = new Message()
            {
                ChannelId = message.ChannelId,
                Checksum = message.Checksum,
                Content = message.Content,
                Sender = user.UserId,
                SentAt = DateTime.UtcNow
            };

            var msg = _context.Message.Add(newMessage).Entity;

            _context.SaveChanges();

            return Ok(new { msg.Content, msg.SentAt, msg.Checksum, msg.Sender, msg.ChannelId });
        }

        [HttpGet("{channelId}")]
        [Authorize]
        public ActionResult GetMessages(long channelId)
        {
            var user = _context.FromIdentity(HttpContext.User);

            if (!_context.Group.Any(e => e.ChannelId == channelId) && !_context.User.Any(e => e.ChannelId == channelId))
                return NotFound();

            var messageUser = _context.User.SingleOrDefault(e => e.ChannelId == channelId);
            var messageGroup = _context.Group.SingleOrDefault(e => e.ChannelId == channelId);

            if (messageGroup != null)
                return Ok(_context.Message.Where(e => e.ChannelId == channelId));

            var messages = _context.Message.Where(e =>
            (e.Sender == user.UserId && e.ChannelId == channelId) ||
            (e.Sender == messageUser.UserId && e.ChannelId == user.ChannelId)).OrderByDescending(e => e.SentAt);

            return Ok(messages);
        }
    }
}
