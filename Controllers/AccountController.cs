using Azure;
using ERP.Helper;
using ERP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using static ERP.Helper.Helper;

namespace ERP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ILogger logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            _logger.LogInformation("Login Api processing");
            _logger.LogInformation("Login Api request "+ System.Text.Json.JsonSerializer.Serialize(model));
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return NotFound(new CommonResponse
                {
                    Code = StatusCodes.Status404NotFound,
                    Message =  { "User not found" },
                });
            }
            if (await _userManager.IsLockedOutAsync(user))
            {
                return BadRequest(new CommonResponse 
                {
                    Code = StatusCodes.Status400BadRequest,
                    Message =  { "Account is locked. Please try again later." },
                });
            }

            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                await _userManager.IsLockedOutAsync(user);
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim("UserName", user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = CreateToken(authClaims);
                var refreshToken = GenerateRefreshToken();

                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

                await _userManager.UpdateAsync(user);

                return Ok(new CommonResponse
                {
                    Code=StatusCodes.Status200OK,
                    Message = {"Login Successfull"},
                    Data= new
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(token),
                        RefreshToken = refreshToken,
                        Expiration = token.ValidTo
                    }
                });
            }
            else
            {
                await _userManager.AccessFailedAsync(user);
                if (await _userManager.IsLockedOutAsync(user))
                {
                    return BadRequest(new CommonResponse
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Message =  { "Account is locked due to multiple failed login attempts." },
                    });
                }
                return Unauthorized(new CommonResponse
                {
                    Code = StatusCodes.Status404NotFound,
                    Message = { "Wrong Password" },
                    
                });
            }
           
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new CommonResponse { Code=StatusCodes.Status422UnprocessableEntity, Message = { "User already exists!" } });

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FristName=model.FirstName,
                LastName=model.LastName,
                Status= UserStatus.New.ToString(),
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new CommonResponse
                {
                    Code = StatusCodes.Status400BadRequest,
                    Message = result.Errors.Select(e => e.Description).ToList()

                });
            }
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }
            var roleResult = await _userManager.AddToRoleAsync(user, UserRoles.User);
            if (!roleResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new CommonResponse
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Message =  { "User role assignment failed!" }
                });
            }

            return Ok(new CommonResponse { Code=StatusCodes.Status200OK, Message = { "User created successfully!" },Data=new { UserName = model.Username } });
        }
        [HttpPost]
        [Route("register_admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new CommonResponse { Code = StatusCodes.Status422UnprocessableEntity, Message = { "User already exists!" } });

            ApplicationUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                FristName = model.FirstName,
                LastName = model.LastName,
                Status = UserStatus.New.ToString(),
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new CommonResponse { Code = StatusCodes.Status500InternalServerError, Message = result.Errors.Select(e => e.Description).ToList() });
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            }
            await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            await _userManager.AddToRoleAsync(user, UserRoles.User);
            

            return Ok(new CommonResponse { Code = StatusCodes.Status200OK, Message = { "User created successfully!" }, Data = new { UserName = model.Username } });
        }

        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

    }
}
