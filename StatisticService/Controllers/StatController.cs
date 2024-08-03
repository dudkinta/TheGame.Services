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
    public class StatController : ControllerBase
    {
        private readonly ILogger<StatController> _logger;
        private readonly IStatisticContext _context;
        private readonly IUserService _userService;

        public StatController(ILogger<StatController> logger, IStatisticContext context, IUserService userService)
        {
            _logger = logger;
            _context = context;
            _userService = userService;
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpGet("check")]
        public async Task<IActionResult> GetStorage(CancellationToken cancellationToken)
        {
            try
            {
                var userId = _userService.GetUserId(User.Claims);

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
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
