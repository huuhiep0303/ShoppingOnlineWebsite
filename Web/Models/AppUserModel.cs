using Microsoft.AspNetCore.Identity;

namespace Web.Models
{
    public class AppUserModel : IdentityUser
    {
        public string Career { get; set; }
        public string RoleID { get; set; }
        public string Token { get; set; }
    }
}
