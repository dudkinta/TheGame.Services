using CommonLibs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatisticDbContext;
using StatisticDbContext.Models;

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
        [HttpPut("setarmy")]
        public async Task<IActionResult> SetArmy(int heroId, int useType, CancellationToken cancellationToken)
        {
            try
            {
                if (useType < 1 || useType > 5)
                    return BadRequest("bad useType");

                var userId = _userService.GetUserId(User.Claims);
                var barrack = await _context.Barracks.FirstOrDefaultAsync(_ => _.user_id == userId && _.id == heroId, cancellationToken);
                var currentArmy = await _context.Armies.Include(_ => _.barrack).Where(_ => _.user_id == userId && _.useType == useType).ToListAsync(cancellationToken);
                if ((useType == 1 && currentArmy.Count < 5) || (useType != 1 && currentArmy.Count == 0))
                {
                    var army = new ArmyModel()
                    {
                        barrack = barrack,
                        barrack_id = heroId,
                        useType = useType,
                        user_id = userId,
                    };
                    await _context.Armies.AddAsync(army, cancellationToken);
                }
                else
                {
                    return BadRequest("Army already full");
                }
                await _context.SaveAsync(cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpPut("firedarmy")]
        public async Task<IActionResult> FiredArmy(int armyId, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _userService.GetUserId(User.Claims);
                var army = await _context.Armies.FirstOrDefaultAsync(_ => _.user_id == userId && _.id == armyId, cancellationToken);
                if (army != null)
                {
                    var equip = await _context.Inventory.Where(_ => _.user_id == userId && _.army_id == army.id).ToListAsync(cancellationToken);
                    foreach (var item in equip)
                    {
                        item.army_id = null;
                        item.army = null;
                    }
                    _context.Armies.Remove(army);
                    await _context.SaveAsync(cancellationToken);
                }
                else
                {
                    return BadRequest("Army already full");
                }
                await _context.SaveAsync(cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpGet("mergeheroes")]
        public async Task<IActionResult> MergeHeroes(CancellationToken cancellationToken)
        {
            try
            {
                var userId = _userService.GetUserId(User.Claims);
                var нeroes = await _context.Barracks.Include(_ => _.hero).Where(_ => _.user_id == userId).ToListAsync(cancellationToken);
                var groupedHeroes = нeroes
                    .GroupBy(h => h.hero!.id)
                    .Where(g => g.Count() >= 3)
                    .OrderBy(g => g.Key).ToList();
                if (groupedHeroes.Count() > 0)
                {
                    foreach (var group in groupedHeroes)
                    {
                        var blankHero = group.FirstOrDefault(_ => _.army != null);
                        if (blankHero == null)
                        {
                            blankHero = group.FirstOrDefault();
                        }
                        if (blankHero != null)
                        {
                            foreach (var item in group)
                            {
                                if (item != blankHero)
                                    _context.Barracks.Remove(item);
                            }
                            var nextHero = await _context.Heroes.FirstOrDefaultAsync(_ => _.level == blankHero.hero!.level + 1 && _.type == blankHero.hero.type, cancellationToken);
                            if (nextHero != null)
                            {
                                blankHero.hero = nextHero;
                                blankHero.hero_id = nextHero.id;

                            }
                        }
                    }
                    await _context.SaveAsync(cancellationToken);
                }
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

        [Authorize(AuthenticationSchemes = "Service", Policy = "ServicePolicy")]
        [HttpGet("gethunter")]
        public async Task<IActionResult> GetHunter(int userId, CancellationToken cancellationToken)
        {
            try
            {
                var hunter = await _context.Armies.Include(_ => _.barrack).Include(_ => _.equip).FirstOrDefaultAsync(_ => _.user_id == userId && _.useType == 1, cancellationToken);
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
