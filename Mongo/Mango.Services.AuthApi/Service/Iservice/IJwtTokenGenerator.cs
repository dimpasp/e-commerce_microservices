using Mango.Services.AuthApi.Models;

namespace Mango.Services.AuthApi.Service.Iservice
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser,IEnumerable<string> roles);
    }
}
