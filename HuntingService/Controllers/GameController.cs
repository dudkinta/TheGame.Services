using CommonLibs;
using ExchangeData;
using ExchangeData.Helpers;
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

                var getHunterEndpoint = _config.GetSection("AppSettings:GetHunterEndpoint").Value ?? string.Empty;
                if (string.IsNullOrEmpty(getHunterEndpoint))
                    return BadRequest("GetHunterEndpoint not found");

                var hunterRespResp = await _apiClient.GetAsync<HuntArmyModel?>($"http://{statisticURL}/{getHunterEndpoint}?userId={userId}", cancellationToken);
                if (!hunterRespResp.IsSuccessStatusCode)
                    return BadRequest(hunterRespResp.Error);

                var coins = aims;
                if (hunterRespResp.Message?.Gun != null)
                {
                    var maxLevelGun = hunterRespResp.Message.Gun.level;
                    if (maxLevelGun > 0)
                        coins = aims * maxLevelGun;
                }
                else
                {
                    coins = aims / 2;
                }

                var rand = new Random(DateTime.UtcNow.Microsecond);
                var heroRewardCount = (int)(Math.Round((0.2 * aims) * rand.NextDouble()));
                var itemRewardCount = (int)(Math.Round((0.1 * aims) * rand.NextDouble()));
                IEnumerable<HeroModel>? heroRewards = null;
                IEnumerable<ItemModel>? itemRewards = null;

                if (hunterRespResp.Message != null)
                {
                    if (hunterRespResp.Message.RewardsHeroes != null && hunterRespResp.Message.RewardsHeroes.Count() > 0)
                        heroRewards = Enumerable.Range(0, heroRewardCount).Select(_ => hunterRespResp.Message.RewardsHeroes.GetRandomElement());

                    if (hunterRespResp.Message.RewardsItems != null && hunterRespResp.Message.RewardsItems.Count() > 0)
                        itemRewards = Enumerable.Range(0, itemRewardCount).Select(_ => hunterRespResp.Message.RewardsItems.GetRandomElement());
                }

                await _messageSender.SendMessage(new FinishHuntModel { Id = userId, AddShots = shots, AddAims = aims, coins = coins, Heroes = heroRewards, Items = itemRewards }, RabbitRoutingKeys.FinishHunt, cancellationToken);
                return Ok(new { main_coins = coins, heroes = heroRewards, items = itemRewards });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest("Error in finishing game");
            }
        }

    }
}
