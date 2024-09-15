using Login.models.Aplication;
using Login.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Login.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize(Roles = "USER")]
    public class UserController : ControllerBase
    {
        private readonly IUserManagerS _userManagerS;

        public UserController(IUserManagerS userManagerS)
        {
            _userManagerS = userManagerS;
        }

        [HttpGet("getCurrentUserInfo")]
        public async Task<ActionResult> GetCurrentUserInfo()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _userManagerS.GetCurrentUser(token);
            return result.Success ? Ok(new {result.StudentCode, result.FirstName , result.LastName ,result.Email }) : Unauthorized(result.Message);
        }
    }
}
