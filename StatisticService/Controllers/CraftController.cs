using CommonLibs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatisticDbContext;

namespace StatisticService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CraftController : ControllerBase
    {
        private readonly ILogger<CraftController> _logger;
        private readonly IStatisticContext _context;
        private readonly IUserService _userService;

        public CraftController(ILogger<CraftController> logger, IStatisticContext context, IUserService userService)
        {
            _logger = logger;
            _context = context;
            _userService = userService;
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
        [HttpGet("create")]
        public async Task<IActionResult> CreateItem(int recipeId, CancellationToken cancellationToken)
        {
            try
            {
                var recipe = await _context.Recipes.FirstOrDefaultAsync(_ => _.id == recipeId, cancellationToken);
                if (recipe == null)
                    return NotFound("Recipe not found");

                var userId = _userService.GetUserId(User.Claims);
                var ingridients = recipe.ingredients.ToDictionary(_ => _.ingredient_id, __ => __.quantity);
                var userIngridients = await _context.Inventory.Where(_ => _.user_id == userId).GroupBy(_ => _.item_id).ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken);
                bool hasAllIngredients = ingridients.All(ingredient =>
                        userIngridients.TryGetValue(ingredient.Key, out int userQuantity) && userQuantity >= ingredient.Value);
                if (!hasAllIngredients)
                    return NotFound("Not enough ingridients");


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
