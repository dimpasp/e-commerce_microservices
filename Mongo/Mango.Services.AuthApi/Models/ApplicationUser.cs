using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthApi.Models
{
    /// <summary>
    ///  ApplicationUser is extenting IdentityUser
    /// </summary>

    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
    }
}
