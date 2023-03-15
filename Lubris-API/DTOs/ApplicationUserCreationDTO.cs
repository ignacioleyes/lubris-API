using Lubris_API.Entities;

namespace Lubris_API.DTOs
{
    public class ApplicationUserCreationDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserTypeEnum UserType { get; set; }

    }
}
