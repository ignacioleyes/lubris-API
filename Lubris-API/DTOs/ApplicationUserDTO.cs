using Lubris_API.Entities;

namespace Lubris_API.DTOs
{
    public class ApplicationUserDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string Password { get; set; }
        public UserTypeEnum UserType { get; set; }
    }
}
