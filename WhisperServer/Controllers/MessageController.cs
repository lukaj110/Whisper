using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using Whisper.DataManager.Models;
using Whisper.DataManager.RequestModels;

namespace WhisperServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly WhisperContext _context;
        private readonly List<WebSocket> _webSockets;
        public MessageController(WhisperContext whisperContext, List<WebSocket> webSocketList)
        {
            _context = whisperContext;
            _webSockets = webSocketList;
        }

        [HttpGet("/ws")]
        public async Task GetStream()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }

            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            _webSockets.Add(webSocket);
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

            var msg = _context.Message.Add(newMessage);

            _context.SaveChanges();

            return Ok(msg.Entity);
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

            return Ok(_context.Message.Where(e => 
            (e.Sender == user.UserId && e.ChannelId == channelId) || 
            (e.Sender == messageUser.UserId && e.ChannelId == user.ChannelId)));
        }
    }
}
