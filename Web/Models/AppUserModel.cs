using Microsoft.AspNetCore.Identity;

namespace Web.Models
{
    public class AppUserModel : IdentityUser
    {
        public string Career { get; set; }
    }
}
