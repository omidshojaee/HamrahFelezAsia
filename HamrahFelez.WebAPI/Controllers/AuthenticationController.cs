using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HamrahFelez.Utilities;
using HamrahFelez.Services;
using HamrahFelez.ViewModels;

namespace HamrahFelez.WebAPI
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly Jwt _jwt;
        
        public AuthenticationController(IUserService userService, Jwt jwt)
        {
            _userService = userService;
            _jwt = jwt;
        }

        [HttpPost]
        [Route("login")]
        [UseProductionDb]
        [AllowAnonymous]

        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                View_WebUser user = await _userService.GetUserByUsernameAsync(new GetUserByUsernameRequest
                {
                    Username = request.Username,
                });

                if (user == null)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Username does not exists.");
                }

                if (!Tools.VerifyHashedPassword(user.HashedPassword, request.Password))
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, "Password is incorrect.");
                }

                if (!user.IsActive)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "Account is not active.");
                }

                var token = _jwt.GenerateToken(user.pkUser, user.fkOptionApiRole);

                var fkAshkhasChilds = new List<long>();

                if (!string.IsNullOrEmpty(user.fkAshkhasChilds))
                {
                    fkAshkhasChilds = user.fkAshkhasChilds
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => long.Parse(x))
                        .ToList();
                }

                var response = new LoginResponse()
                {
                    Token = token,
                    fkAshkhasChilds = fkAshkhasChilds
                };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
