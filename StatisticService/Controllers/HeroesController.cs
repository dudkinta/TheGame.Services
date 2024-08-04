using CommonLibs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatisticDbContext;

namespace StatisticService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HeroesController : ControllerBase
    {
        private readonly ILogger<HeroesController> _logger;
        private readonly IStatisticContext _context;
        private readonly IUserService _userService;

        public HeroesController(ILogger<HeroesController> logger, IStatisticContext context, IUserService userService)
        {
            _logger = logger;
            _context = context;
            _userService = userService;
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpGet("getheroes")]
        public async Task<IActionResult> GetHeroes(CancellationToken cancellationToken)
        {
            try
            {
                var userId = _userService.GetUserId(User.Claims);
                var heroes = await _context.Barracks
                                            .Include(_ => _.army).ThenInclude(_ => _.equip).ThenInclude(_ => _.item).Include(_ => _.hero)
                                            .Where(_ => _.user_id == userId).ToListAsync();
                return Ok(heroes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = "Service", Policy = "ServicePolicy")]
        [HttpGet("gethunter")]
        public async Task<IActionResult> GetHunter(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var hunter = await _context.Armies.Include(_ => _.barrack).Include(_ => _.equip).Where(_ => _.user_id == userId && _.useType == 1).FirstOrDefaultAsync(cancellationToken);
                var items = _context.Items;
                var heroes = _context.Heroes;
                if (hunter == null)
                    return Ok(new
                    {
                        RewardsItems = await items.Where(_ => _.level == 1).ToListAsync(cancellationToken),
                        RewardsHeroes = await heroes.Where(_ => _.level == 1).ToListAsync(cancellationToken)
                    });

                if (hunter.barrack == null)
                    return BadRequest("Bad hunter army");

                var hero = await _context.Heroes.FirstOrDefaultAsync(_ => _.id == hunter.barrack.hero_id, cancellationToken);
                if (hero == null)
                    return BadRequest("Hero in barrack error");

                var itemsId = hunter.equip?.Select(_ => _.item_id).ToList();
                if (itemsId == null || itemsId.Count() == 0)
                    return Ok(new
                    {
                        Hero = hero,
                        RewardsItems = await items.Where(_ => _.level == 1).ToListAsync(cancellationToken),
                        RewardsHeroes = await heroes.Where(_ => _.level == 1 || _.level < hero.level - 5).ToListAsync(cancellationToken)
                    });

                var gun = await _context.Items.Where(_ => itemsId.Contains(_.id) && _.type == "gun").FirstOrDefaultAsync(cancellationToken);
                if (gun == null)
                    return Ok(new
                    {
                        Hero = hero,
                        RewardsItems = await items.Where(_ => _.level == 1).ToListAsync(cancellationToken),
                        RewardsHeroes = await heroes.Where(_ => _.level == 1 || _.level < hero.level - 5).ToListAsync(cancellationToken)
                    });

                return Ok(new
                {
                    Hero = hero,
                    Gun = gun,
                    RewardsItems = await items.Where(_ => _.level == 1 || _.level < gun.level - 5).ToListAsync(),
                    RewardsHeroes = await heroes.Where(_ => _.level == 1 || _.level < hero.level - 5).ToListAsync()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
