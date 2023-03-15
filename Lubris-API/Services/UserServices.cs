using AutoMapper;
using Lubris_API.DTOs;
using Lubris_API.Entities;
using Lubris_API.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using static Lubris_API.Utils.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lubris_API.Services.Interfaces;
using Lubris_API.Extensions;

namespace Lubris_API.Services
{
    public class UserServices: ControllerBase, IUserServices
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly IActionContextAccessor contextAccessor;

        public UserServices(
            ApplicationDbContext context,
            IMapper mapper, 
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IActionContextAccessor contextAccessor)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.contextAccessor = contextAccessor;
        }

        public async Task<ActionResult<AuthResponseDTO>> CreateUser(ApplicationUserCreationDTO creationDTO)
        {
            var user = new ApplicationUser
            {
                UserName = creationDTO.Email,
                Email = creationDTO.Email,
                FirstName = creationDTO.FirstName,
                LastName = creationDTO.LastName,
                UserType = creationDTO.UserType
            };
            var result = await userManager.CreateAsync(user, creationDTO.Password);

            if (result.Succeeded)
            { 
                return await BuildToken(user);
            }

            return BadRequest(result.Errors);
        }

        public async Task<ActionResult<ListResponse<ApplicationUserDTO>>> GetUsers(UserFilter filter)
        {
            var queryable = context.ApplicationUsers.AsQueryable();
            if(!string.IsNullOrEmpty(filter.GeneralSearch))
            {
                queryable = queryable
                    .Where(q => 
                        q.FullName.Contains(filter.GeneralSearch) ||
                        q.Email.Contains(filter.GeneralSearch));
            }

            return await queryable.FilterSortPaginate<ApplicationUser, ApplicationUserDTO>(filter, mapper, contextAccessor);
        }

        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            var user = await userManager.FindByEmailAsync(loginDTO.Email);

            if (user == null)
            {
                return BadRequest(BuildErrorResponse("EMAIL_NOT_REGISTERED", "Email not registered"));
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

            if (result.Succeeded)
            {
                return await BuildToken(user);
            }

            return BadRequest(BuildErrorResponse("PASSWORD_INVALID", "Invalid password"));
        }



        private async Task<AuthResponseDTO> BuildToken(ApplicationUser user)
        {
            var claims = new List<Claim>()
        {
            new (CustomClaims.ID_CLAIM_TYPE, user.Id),
            new (CustomClaims.EMAIL_CLAIM_TYPE, user.Email)
        };

            switch (user.UserType)
            {
                case UserTypeEnum.Admin:
                    claims.Add(new Claim(ClaimTypes.Role, Roles.ADMIN));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var claimsDB = await userManager.GetClaimsAsync(user);
            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("LUBRIS_JWT_KEY")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var validDays = 7;
            var expiration = DateTime.UtcNow.AddDays(validDays);

            var authToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiration.ToLocalTime(), signingCredentials: credentials);

            var response = new AuthResponseDTO
            {
                AuthToken =
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(authToken),
                    ExpiresIn = (int)(expiration - DateTime.UtcNow).TotalMinutes
                },
                TokenType = "Bearer",
                AuthState = mapper.Map<ApplicationUserDTO>(user)
            };
            return response;
        }

        private AuthResponseDTO BuildErrorResponse(string errorCode, string errorDescription)
        {
            return new AuthResponseDTO
            {
                Error = new ApiError
                {
                    Code = errorCode,
                    Description = errorDescription
                }
            };
        }
    }
}
