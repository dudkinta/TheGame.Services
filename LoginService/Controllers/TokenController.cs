using Consul;
using ExchangeData;
using ExchangeData.Interfaces;
using ExchangeData.Models;
using LoginDbContext;
using LoginDbContext.Models;
using LoginService.Helpers;
using LoginService.Interfaces;
using LoginService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace LoginService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ILogger<TokenController> _logger;
        private readonly ITokenService _tokenService;
        private readonly ILoginContext _context;
        private readonly IMessageSender _messageSender;
        private readonly IConsulClient _consulClient;

        public TokenController(ILogger<TokenController> logger, ITokenService tokenService, ILoginContext context, IMessageSender messageSender, IConsulClient consulClient)
        {
            _logger = logger;
            _tokenService = tokenService;
            _context = context;
            _messageSender = messageSender;
            _consulClient = consulClient;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] TWAModel twa, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(twa?.initData))
                return BadRequest("Bad init data");

            var hash = twa!.initDataUnsafe?.hash!;
            if (string.IsNullOrEmpty(hash))
                return BadRequest("Bad hash data");

            var tokenPair = await _consulClient.KV.Get("private/gametoken");
            if (tokenPair.Response == null)
                return BadRequest("Key not found in Consul");

            var token = Encoding.UTF8.GetString(tokenPair.Response.Value, 0, tokenPair.Response.Value.Length);
            if (string.IsNullOrEmpty(token))
                return BadRequest("Bad token");

            var initDataList = WebUtility.UrlDecode(twa.initData)
                .Split('&')
                .Where(chunk => !chunk.StartsWith("hash="))
                .Select(chunk => chunk.Split('='))
                .OrderBy(rec => rec[0])
                .ToList();

            long referId = 0;
            var startParam = twa!.initDataUnsafe?.start_param;
            if (!string.IsNullOrEmpty(startParam))
            {
                try
                {
                    referId = Convert.ToInt64(startParam, 16);
                }
                catch (Exception)
                {
                    return BadRequest("Error parse referal code");
                }
            }

            var res = Helper.Validate(hash, initDataList, token);
            if (res)
            {
                var userData = initDataList.FirstOrDefault(_ => _[0] == "user");
                if (userData != null)
                {
                    var user = JsonConvert.DeserializeObject<TWAUserModel>(userData[1]);
                    if (user == null)
                        return BadRequest("user is null");

                    var db_user = await _context.Users.FirstOrDefaultAsync(_ => _.tg_id == user.id, cancellationToken);
                    if (db_user == null)
                    {
                        var new_user = _context.Users.Add(new UserModel
                        {
                            tg_id = user.id,
                            first_name = user.first_name,
                            last_name = user.last_name,
                            allows_write_to_pm = user.allows_write_to_pm,
                            is_premium = user.is_premium,
                            language_code = user.language_code,
                            last_login = DateTime.UtcNow,
                            username = user.username
                        });
                        await _context.SaveAsync(cancellationToken);
                        db_user = new_user.Entity;
                    }
                    if (referId != 0)
                    {
                        await _messageSender.SendMessage(new ReferModel { Id = db_user.id, Refer_id = referId }, RabbitRoutingKeys.Referal, cancellationToken);
                    }
                    var jwt = _tokenService.GenerateUserToken(db_user, new TimeSpan(1, 0, 0));
                    return Ok(new { token = jwt });
                }
            }
            return BadRequest("Bad data");
        }
    }
}
