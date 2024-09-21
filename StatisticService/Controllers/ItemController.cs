using CommonLibs;
using ExchangeData.Models;
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
        [HttpGet("getallitems")]
        public async Task<IActionResult> GetItems(CancellationToken cancellationToken)
        {
            try
            {
                var userId = _userService.GetUserId(User.Claims);
                var inventory = await _context.Inventory.Include(_ => _.item).Where(_ => _.user_id == userId).ToListAsync(cancellationToken);
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpGet("getitems")]
        public async Task<IActionResult> GetItems(string itemType, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _userService.GetUserId(User.Claims);
                var inventory = await _context.Inventory.Include(_ => _.item).Where(_ => _.user_id == userId && _.item!.type == itemType).ToListAsync(cancellationToken);
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpPost("up")]
        public async Task<IActionResult> UpItem(ChangeItemModel changeItem, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _userService.GetUserId(User.Claims);
                var invItem = await _context.Inventory.Include(_ => _.item).FirstOrDefaultAsync(_ => _.user_id == userId && _.id == changeItem.item_id, cancellationToken);
                if (invItem == null)
                    return BadRequest("Not Item NotFound");

                var currentItem = await _context.Inventory.Include(_ => _.item).FirstOrDefaultAsync(_ => _.user_id == userId && _.army_id == changeItem.army_id && _.item!.type == invItem.item!.type, cancellationToken);
                if (currentItem != null)
                {
                    currentItem.army_id = null;
                }
                invItem.army_id = changeItem.army_id;
                await _context.SaveAsync(cancellationToken);
                var heroes = await _context.Barracks
                                            .Include(_ => _.army).ThenInclude(_ => _.equip).ThenInclude(_ => _.item).Include(_ => _.hero)
                                            .Where(_ => _.user_id == userId).ToListAsync(cancellationToken);
                return Ok(heroes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpPost("down")]
        public async Task<IActionResult> DownItem(ChangeItemModel changeItem, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _userService.GetUserId(User.Claims);
                var invItem = await _context.Inventory.Include(_ => _.item).FirstOrDefaultAsync(_ => _.user_id == userId && _.id == changeItem.item_id, cancellationToken);
                if (invItem == null)
                    return BadRequest("Not Item NotFound");

                invItem.army_id = null;
                await _context.SaveAsync(cancellationToken);
                var heroes = await _context.Barracks
                                            .Include(_ => _.army).ThenInclude(_ => _.equip).ThenInclude(_ => _.item).Include(_ => _.hero)
                                            .Where(_ => _.user_id == userId).ToListAsync(cancellationToken);
                return Ok(heroes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
