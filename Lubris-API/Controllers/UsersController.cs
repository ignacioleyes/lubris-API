using Lubris_API.DTOs;
using Lubris_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lubris_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController: ControllerBase
    {
        private readonly IUserServices userServices;

        public UsersController(IUserServices userServices)
        {
            this.userServices = userServices;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<AuthResponseDTO>> CreateUser([FromBody] ApplicationUserCreationDTO creationDTO)
        {
            return await userServices.CreateUser(creationDTO);
        }

        [HttpGet ("GetAllUsers")]
        public async Task<ActionResult<ListResponse<ApplicationUserDTO>>> GetUsers([FromQuery] UserFilter filter)
        {
            return await userServices.GetUsers(filter);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            return await userServices.Login(loginDTO);
        }

    }
}
