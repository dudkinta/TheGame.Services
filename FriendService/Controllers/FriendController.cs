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

        public FriendController(ILogger<FriendController> logger, IConfiguration config, IFriendContext context)
        {
            _logger = logger;
            _config = config;
            _context = context;
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpGet("getlink")]
        public IActionResult GetReferLink(CancellationToken cancellationToken)
        {
            var baseLink = _config.GetSection("AppSettings:GameURL").Value ?? string.Empty;
            if (string.IsNullOrEmpty(baseLink))
                return BadRequest("Refer Link generate error");

            var user = User;
            var tg_id = user.Claims.FirstOrDefault(_ => _.Type == "tg_id")?.Value;
            if (string.IsNullOrEmpty(tg_id))
                return BadRequest("Telegram user is corrupt");

            if (long.TryParse(tg_id, out long parse_id))
            {
                var hexString = parse_id.ToString("X");
                return Ok(new { ReferalLink = baseLink + hexString });
            }
            else
            {
                return BadRequest("Telegram user is corrupt");
            }
        }

        [Authorize(AuthenticationSchemes = "User", Policy = "UserPolicy")]
        [HttpGet("getfriends")]
        public async Task<IActionResult> GetFriends(IInnerApiClient apiClient, ConsulServiceDiscovery serviceDiscovery, CancellationToken cancellationToken)
        {
            try
            {
                var user = User;
                var id_str = user.Claims.FirstOrDefault(_ => _.Type == "tg_id")?.Value;

                if (!string.IsNullOrEmpty(id_str))
                {
                    if (long.TryParse(id_str, out long tg_id))
                    {
                        var refers = await _context.Friends.Where(_ => _.refer_id == tg_id).Select(_ => new { _.id }).ToListAsync(cancellationToken);
                        var json = JsonConvert.SerializeObject(refers.Select(_ => _.id).ToArray());
                        var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                        var friendURL = await serviceDiscovery.GetServiceAddress("LoginService");
                        if (string.IsNullOrEmpty(friendURL))
                            return NotFound("Service Friend not found");

                        var friendEndpoint = _config.GetSection("AppSettings:GetFriendNameEndpoint").Value ?? string.Empty;

                        if (!string.IsNullOrEmpty(friendEndpoint))
                        {
                            var friends = await apiClient.PostAsync<IEnumerable<FriendNamesModel>>($"http://{friendURL}/{friendEndpoint}", httpContent, cancellationToken);
                            if (friends.IsSuccessStatusCode)
                                return Ok(friends.Message);
                            else
                                return BadRequest(friends.Error);
                        }
                        else
                        {
                            return BadRequest("GetFriendEndpoint not found");
                        }

                    }
                    else
                    {
                        return BadRequest("Telegram user is corrupt");
                    }
                }
                else
                {
                    return BadRequest("Telegram user is corrupt");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
