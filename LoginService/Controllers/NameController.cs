using ExchangeData.Models;
using LoginDbContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoginService.Controllers
{
    [Authorize(AuthenticationSchemes = "Service", Policy = "ServicePolicy")]
    [ApiController]
    [Route("[controller]")]
    public class NameController : ControllerBase
    {
        private readonly ILogger<TokenController> _logger;
        private readonly ILoginContext _context;

        public NameController(ILogger<TokenController> logger, ILoginContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost("getnames")]
        public async Task<IActionResult> GetNames(int[] ids, CancellationToken cancellationToken)
        {
            try
            {
                var db_users = await _context.Users.Where(_ => ids.Contains(_.id)).ToListAsync(cancellationToken);
                return Ok(db_users.Select(_ => new FriendNamesModel { Id = _.id, UserName = _.username, FirstName = _.first_name, LastName = _.last_name }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
