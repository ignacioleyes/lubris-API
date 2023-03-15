using Lubris_API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Lubris_API.Services.Interfaces
{
    public interface IUserServices
    {
        Task<ActionResult<AuthResponseDTO>> CreateUser(ApplicationUserCreationDTO creationDTO);
        Task<ActionResult<ListResponse<ApplicationUserDTO>>> GetUsers(UserFilter filter);
        Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO loginDTO);
    }
}
