using ExchangeData.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatisticDbContext;
using StatisticDbContext.Models;

namespace StatisticService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatController : ControllerBase
    {
        private readonly ILogger<StatController> _logger;
        private readonly IConfiguration _config;
        private readonly IStatisticContext _context;
        private readonly IMessageSender _messageSender;

        public StatController(ILogger<StatController> logger, IConfiguration config, IStatisticContext context, IMessageSender messageSender)
        {
            _logger = logger;
            _config = config;
            _context = context;
            _messageSender = messageSender;
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpGet("check")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var userIdStr = User.Claims.FirstOrDefault(_ => _.Type == "id")?.Value;
            var userId = 0;
            if (string.IsNullOrEmpty(userIdStr))
                return BadRequest("userId not found");

            if (!Int32.TryParse(userIdStr, out userId))
                return BadRequest("userId is bad");

            try
            {
                var userStats = await _context.Storage.FirstOrDefaultAsync(_ => _.user_id == userId, cancellationToken);
                if (userStats == null)
                {
                    userStats = new StorageModel()
                    {
                        user_id = userId,
                        last_check_energy = DateTime.UtcNow,
                        energy = 600,
                        bonus_coin = 0,
                        main_coin = 0,
                        refer_coin = 0,
                        task_coin = 0,
                    };
                    _context.Storage.Add(userStats);
                    await _context.SaveAsync(cancellationToken);
                }
                var addEnergy = (int)Math.Round((DateTime.UtcNow - userStats.last_check_energy).TotalSeconds);
                userStats.energy += addEnergy;
                userStats.last_check_energy = DateTime.UtcNow;
                if (userStats.energy > 6000) { userStats.energy = 6000; }
                await _context.SaveAsync(cancellationToken);
                return Ok(userStats);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = "Service", Policy = "ServicePolicy")]
        [HttpGet("getenergy")]
        public async Task<IActionResult> GetEnergy(int userId, int amount, CancellationToken cancellationToken)
        {
            try
            {
                var userStats = await _context.Storage.FirstOrDefaultAsync(_ => _.user_id == userId, cancellationToken);
                if (userStats == null)
                    return BadRequest("User not found");

                var addEnergy = (int)Math.Round((DateTime.UtcNow - userStats.last_check_energy).TotalSeconds);
                userStats.energy += addEnergy;
                if (userStats.energy > 6000) { userStats.energy = 6000; }
                userStats.energy -= amount;
                if (userStats.energy >= 0)
                {
                    await _context.SaveAsync(cancellationToken);
                    return Ok(true);
                }
                return BadRequest("Energy not enough");
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
                        Items = await items.Where(_ => _.level == 1).ToListAsync(cancellationToken),
                        Heroes = await heroes.Where(_ => _.level == 1).ToListAsync(cancellationToken)
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
                        Items = await items.Where(_ => _.level == 1).ToListAsync(cancellationToken),
                        Heroes = await heroes.Where(_ => _.level == 1 || _.level < hero.level - 5).ToListAsync(cancellationToken)
                    });

                var gun = await _context.Items.Where(_ => itemsId.Contains(_.id) && _.type == "gun").FirstOrDefaultAsync(cancellationToken);
                if (gun == null)
                    return Ok(new
                    {
                        Hero = hero,
                        Items = await items.Where(_ => _.level == 1).ToListAsync(cancellationToken),
                        Heroes = await heroes.Where(_ => _.level == 1 || _.level < hero.level - 5).ToListAsync(cancellationToken)
                    });

                return Ok(new
                {
                    Hero = hero,
                    Gun = gun,
                    Items = await items.Where(_ => _.level == 1 || _.level < gun.level - 5).ToListAsync(),
                    Heroes = await heroes.Where(_ => _.level == 1 || _.level < hero.level - 5).ToListAsync()
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
