using Microsoft.AspNetCore.Identity;

namespace MagicVilla_WebAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        // we dont have that in built in identity user
        public int Name { get; set; }
    }
}
