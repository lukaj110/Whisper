using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Whisper.DataManager.Models;

namespace WhisperServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly WhisperContext _context;
        public MessageController(WhisperContext whisperContext) => _context = whisperContext;
    }
}
