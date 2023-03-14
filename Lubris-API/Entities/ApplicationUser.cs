using Microsoft.AspNetCore.Identity;

namespace Lubris_API.Entities
{
    public class ApplicationUser: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Password { get; set; }
        public UserTypeEnum UserType { get; set; }

    }

    public enum UserTypeEnum
    {
        Admin,
    }
}
