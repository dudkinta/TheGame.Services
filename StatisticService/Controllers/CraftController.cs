using CommonLibs;
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
    public class CraftController : ControllerBase
    {
        private readonly ILogger<CraftController> _logger;
        private readonly IStatisticContext _context;
        private readonly IUserService _userService;
        private readonly IMessageSender _messageSender;

        public CraftController(ILogger<CraftController> logger, IStatisticContext context, IUserService userService, IMessageSender messageSender)
        {
            _logger = logger;
            _context = context;
            _userService = userService;
            _messageSender = messageSender;
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpGet("getrecipes")]
        public async Task<IActionResult> GetRecipes(CancellationToken cancellationToken)
        {
            try
            {
                var recipes = await _context.Recipes.Include(_ => _.item).Include(_ => _.ingredients).ThenInclude(_ => _.ingredient).ToListAsync(cancellationToken);
                return Ok(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpPut("create")]
        public async Task<IActionResult> CreateItem(int recipeId, CancellationToken cancellationToken)
        {
            try
            {
                var recipe = await _context.Recipes.Include(_ => _.ingredients).FirstOrDefaultAsync(_ => _.id == recipeId, cancellationToken);
                if (recipe == null)
                    return NotFound("Recipe not found");

                var userId = _userService.GetUserId(User.Claims);
                var ingridients = recipe.ingredients.ToDictionary(_ => _.ingredient_id, __ => __.quantity);
                var userIngridients = await _context.Inventory.Where(_ => _.user_id == userId && _.army_id == null).GroupBy(_ => _.item_id).ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken);
                bool hasAllIngredients = ingridients.All(ingredient =>
                        userIngridients.TryGetValue(ingredient.Key, out int userQuantity) && userQuantity >= ingredient.Value);
                if (!hasAllIngredients)
                    return NotFound("Not enough ingridients");

                foreach (var ingredient in ingridients)
                {
                    var requiredQuantity = ingredient.Value;
                    var inventoryItems = await _context.Inventory
                        .Where(_ => _.user_id == userId && _.item_id == ingredient.Key)
                        .Take(requiredQuantity)
                        .ToListAsync(cancellationToken);

                    if (inventoryItems.Count < requiredQuantity)
                    {
                        return NotFound("Not enough ingredients to remove");
                    }

                    _context.Inventory.RemoveRange(inventoryItems);
                }
                await _context.Crafts.AddAsync(new CraftModel()
                {
                    dt_end = DateTime.UtcNow.AddSeconds(recipe.craft_time),
                    user_id = userId,
                    recire_id = recipeId,
                }, cancellationToken);
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
        [HttpGet("check")]
        public async Task<IActionResult> CheckCrafts(CancellationToken cancellationToken)
        {
            try
            {
                var userId = _userService.GetUserId(User.Claims);
                var crafts = await _context.Crafts.Where(_ => _.user_id == userId).ToListAsync(cancellationToken);
                return Ok(crafts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpPut("give")]
        public async Task<IActionResult> GiveCrafts(int craftId, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _userService.GetUserId(User.Claims);
                var craft = await _context.Crafts.Where(_ => _.user_id == userId && _.id == craftId).FirstOrDefaultAsync(cancellationToken);
                if (craft == null)
                    return NotFound("Craft not found");

                if (craft.dt_end > DateTime.UtcNow)
                    return BadRequest("Craft not completed");

                await _messageSender.SendMessage(craft, ExchangeData.RabbitRoutingKeys.Craft, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
