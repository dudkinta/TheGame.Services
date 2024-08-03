using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatisticDbContext;

namespace StatisticService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnergyController : ControllerBase
    {
        private readonly ILogger<EnergyController> _logger;
        private readonly IStatisticContext _context;

        public EnergyController(ILogger<EnergyController> logger, IStatisticContext context)
        {
            _logger = logger;
            _context = context;
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
    }
}
