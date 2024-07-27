using CommonLibs;
using ExchangeData;
using ExchangeData.Interfaces;
using ExchangeData.Models;
using HuntingDbContext;
using HuntingDbContext.Models;
using HuntingService.Models;
using InnerApiLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HuntingService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private const int maxAnimals = 25;
        private const int maxTargets = 5;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly IMessageSender _messageSender;
        private readonly IInnerApiClient _apiClient;
        private readonly IHuntingContext _context;
        private readonly ConsulServiceDiscovery _serviceDiscovery;

        public GameController(ILogger<GameController> logger, IConfiguration config, IMessageSender messageSender, IInnerApiClient apiClient,
            IHuntingContext context, ConsulServiceDiscovery serviceDiscovery)
        {
            _logger = logger;
            _config = config;
            _messageSender = messageSender;
            _apiClient = apiClient;
            _context = context;
            _serviceDiscovery = serviceDiscovery;
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpGet("getgame")]
        public async Task<IActionResult> GetGame(CancellationToken cancellationToken)
        {
            try
            {
                var userIdStr = User.Claims.FirstOrDefault(_ => _.Type == "id")?.Value;
                var userId = 0;
                if (string.IsNullOrEmpty(userIdStr))
                    return BadRequest("userId not found");

                if (!Int32.TryParse(userIdStr, out userId))
                    return BadRequest("userId is bad");

                var game = await _context.Games.FirstOrDefaultAsync(_ => _.user_id == userId && _.status == 0, cancellationToken);
                if (game != null)
                    return Ok(game);


                var statisticURL = await _serviceDiscovery.GetServiceAddress("StatisticService");
                if (string.IsNullOrEmpty(statisticURL))
                    return NotFound("Service Statistics not found");

                var getEnergyEndpoint = _config.GetSection("AppSettings:GetEnergyEndpoint").Value ?? string.Empty;
                if (string.IsNullOrEmpty(getEnergyEndpoint))
                    return BadRequest("GetStatisticEndpoint not found");

                var energyResp = await _apiClient.GetAsync<bool>($"http://{statisticURL}/{getEnergyEndpoint}?userId={userId}&amount=600", cancellationToken);
                if (!energyResp.IsSuccessStatusCode)
                    return BadRequest(energyResp.Error);

                var rand = new Random(DateTime.Now.Microsecond);
                var targets = Enumerable.Range(0, maxAnimals).Select(_ => (byte)rand.Next(maxTargets)).ToArray();

                game = new GameModel()
                {
                    game_guid = Guid.NewGuid(),
                    status = 0,
                    user_id = userId,
                    targets = Convert.ToBase64String(targets)
                };
                await _context.Games.AddAsync(game, cancellationToken);
                await _context.SaveAsync(cancellationToken);
                return Ok(game);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest("Error in getting game");
            }
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpPost("finishgame")]
        public async Task<IActionResult> FinishGame(GameResult gameResult, CancellationToken cancellationToken)
        {
            try
            {
                var userIdStr = User.Claims.FirstOrDefault(_ => _.Type == "id")?.Value;
                var userId = 0;
                if (string.IsNullOrEmpty(userIdStr))
                    return BadRequest("userId not found");

                if (!Int32.TryParse(userIdStr, out userId))
                    return BadRequest("userId is bad");

                if (string.IsNullOrEmpty(gameResult.GameId) || !Guid.TryParse(gameResult.GameId, out Guid gameGuid))
                    return BadRequest("GameId is incorrupt");

                var game = await _context.Games.FirstOrDefaultAsync(_ => _.user_id == userId && _.status == 0 && _.game_guid == gameGuid, cancellationToken);
                if (game == null)
                    return BadRequest("GameId not found");

                game.status = 1;
                await _context.SaveAsync(cancellationToken);

                if (string.IsNullOrEmpty(game.targets))
                    return BadRequest("Game targets is null");

                var targets = Convert.FromBase64String(game.targets);
                var aims = 0;
                var shots = gameResult.Shots;
                for (var i = 0; i < targets.Length; i++)
                {
                    var lineCheck = gameResult.AimCount?.FirstOrDefault(_ => _.id == i + 1);
                    if (lineCheck != null)
                    {
                        if (lineCheck.Count > targets[i])
                            return BadRequest("Error in check targets");

                        aims += lineCheck.Count;
                    }
                }
                var statisticURL = await _serviceDiscovery.GetServiceAddress("StatisticService");
                if (string.IsNullOrEmpty(statisticURL))
                    return NotFound("Service Statistics not found");

                var getItemsEndpoint = _config.GetSection("AppSettings:GetItemsEndpoint").Value ?? string.Empty;
                if (string.IsNullOrEmpty(getItemsEndpoint))
                    return BadRequest("GetItemsEndpoint not found");

                var itemsResp = await _apiClient.GetAsync<IEnumerable<InventoryModel>?>($"http://{statisticURL}/{getItemsEndpoint}?userId={userId}&itemType=gun", cancellationToken);
                if (!itemsResp.IsSuccessStatusCode)
                    return BadRequest(itemsResp.Error);

                var coins = aims;
                var guns = itemsResp.Message?.Where(_ => _.item != null).Select(_ => _.item);
                if (guns != null && guns.Count() > 0)
                {
                    var maxLevelGun = guns.Max(_ => _!.level);
                    if (maxLevelGun > 0)
                        coins = aims * maxLevelGun;
                }
                await _messageSender.SendMessage(new FinishHuntModel { Id = userId, AddShots = shots, AddAims = aims, coins = coins }, RabbitRoutingKeys.FinishHunt, cancellationToken);
                return Ok(new { coins = coins });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest("Error in finishing game");
            }
        }

    }
}
