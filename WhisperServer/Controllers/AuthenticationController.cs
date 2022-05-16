using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Whisper.DataManager.Models;

namespace WhisperServer.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly WhisperContext _context;

        public AuthenticationController() => _context = new WhisperContext();
    }
}
