using CommonLibs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatisticDbContext;

namespace StatisticService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArmyController : ControllerBase
    {
        private readonly ILogger<ArmyController> _logger;
        private readonly IStatisticContext _context;
        private readonly IUserService _userService;

        public ArmyController(ILogger<ArmyController> logger, IStatisticContext context, IUserService userService)
        {
            _logger = logger;
            _context = context;
            _userService = userService;
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpGet("getarmy")]
        [Obsolete]
        public async Task<IActionResult> GetArmy(int armyId, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _userService.GetUserId(User.Claims);
                var army = await _context.Armies
                                        .Include(a => a.equip)
                                            .ThenInclude(e => e.item)
                                        .FirstOrDefaultAsync(a => a.user_id == userId && a.id == armyId);
                return Ok(army);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
