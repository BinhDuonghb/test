using Amazon.Runtime.Internal;
using Login.models.Aplication;
using Login.models.Request;
using Login.models.setting;
using Login.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Login.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly IUserManagerS _userManagerService;
        private readonly RoleManager<ARole> _roleManager;
        private readonly ILogger<OperationsController> _logger;

        public OperationsController(IUserManagerS userManagerService, RoleManager<ARole> roleManager, ILogger<OperationsController> logger)
        {
            _userManagerService = userManagerService;
            _roleManager = roleManager;
            _logger = logger;
        }
        [HttpPost("addRole")]
        public async Task<ActionResult> CreateRole([FromBody] CreateRoleRequest createRoleRequest)
        {
            ARole newRole = new() { Name = createRoleRequest.RoleName };
            var result = await _roleManager.CreateAsync(newRole);
            if (!result.Succeeded)
            {
                _logger.LogError(result.Errors.First().Description);
                return BadRequest(new { Message = $"Create role fail  {result?.Errors?.First()?.Description}" });
            }
            _logger.LogInformation("Crate role.");
            return Ok(new { Message = "Create role success" });
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> CreateUser(RegisterRequest registerRequest)
        {
            var result = await _userManagerService.Register(registerRequest);
            _logger.LogInformation("User has register.");
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(LoginRequest loginRequest)
        {
            var result = await _userManagerService.Login(loginRequest);
            _logger.LogInformation("User logged in.");
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpPost]
        [Route("refreshToken")]
        public async Task<ActionResult> RefreshToken(RefreshTokenRequest request)
        {
            var result = await _userManagerService.RefreshToken(request);
            _logger.LogInformation("User Token is refreshed.");
            return result.Success ? Ok(result) : Unauthorized(result);
        }
    }
}
