using CommonLibs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatisticDbContext;

namespace StatisticService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;
        private readonly IStatisticContext _context;
        private readonly IUserService _userService;

        public ItemController(ILogger<ItemController> logger, IStatisticContext context, IUserService userService)
        {
            _logger = logger;
            _context = context;
            _userService = userService;
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpGet("getitems")]
        [Obsolete]
        public async Task<IActionResult> GetItems(string itemType, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _userService.GetUserId(User.Claims);
                var inventory = await _context.Inventory.Include(_ => _.item).Where(_ => _.user_id == userId && _.item!.type == itemType).ToListAsync();
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
