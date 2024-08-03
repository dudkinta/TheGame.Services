using CommonLibs;
using ExchangeData.Models;
using FriendDbContex;
using InnerApiLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace FriendService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FriendController : ControllerBase
    {
        private readonly ILogger<FriendController> _logger;
        private readonly IConfiguration _config;
        private readonly IFriendContext _context;
        private readonly IUserService _userService;

        public FriendController(ILogger<FriendController> logger, IConfiguration config, IFriendContext context, IUserService userService)
        {
            _logger = logger;
            _config = config;
            _context = context;
            _userService = userService;
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpGet("getlink")]
        public IActionResult GetReferLink(CancellationToken cancellationToken)
        {
            try
            {
                var baseLink = _config.GetSection("AppSettings:GameURL").Value ?? string.Empty;
                if (string.IsNullOrEmpty(baseLink))
                    return BadRequest("Refer Link generate error");

                var tg_id = _userService.GetTelegramId(User.Claims);
                return Ok(new { ReferalLink = baseLink + tg_id.ToString("X") });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpGet("getfriends")]
        public async Task<IActionResult> GetFriends(IInnerApiClient apiClient, ConsulServiceDiscovery serviceDiscovery, CancellationToken cancellationToken)
        {
            try
            {
                var tg_id = _userService.GetTelegramId(User.Claims);

                var refers = await _context.Friends.Where(_ => _.refer_id == tg_id).Select(_ => new { _.id }).ToListAsync(cancellationToken);
                var json = JsonConvert.SerializeObject(refers.Select(_ => _.id).ToArray());
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var friendURL = await serviceDiscovery.GetServiceAddress("LoginService");
                if (string.IsNullOrEmpty(friendURL))
                    return NotFound("Service Friend not found");

                var friendEndpoint = _config.GetSection("AppSettings:GetFriendNameEndpoint").Value ?? string.Empty;

                if (string.IsNullOrEmpty(friendEndpoint))
                    return BadRequest("GetFriendEndpoint not found");

                var friends = await apiClient.PostAsync<IEnumerable<FriendNamesModel>>($"http://{friendURL}/{friendEndpoint}", httpContent, cancellationToken);
                if (friends.IsSuccessStatusCode)
                    return Ok(friends.Message);
                else
                    return BadRequest(friends.Error);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
