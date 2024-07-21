using InnerTokenService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InnerTokenService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ILogger<TokenController> _logger;
        private readonly ITokenService _tokenService;
        public TokenController(ILogger<TokenController> logger, IConfiguration config, ITokenService tokenService)
        {
            _logger = logger;
            _tokenService = tokenService;
        }

        [HttpGet("login")]
        public IActionResult Login(CancellationToken cancellationToken)
        {
            var jwt = _tokenService.GenerateServiceToken(new TimeSpan(24, 0, 0));
            return Ok(new { token = jwt });
        }
    }
}
