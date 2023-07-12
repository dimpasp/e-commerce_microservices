using Mango.Services.AuthApi.Models;
using Mango.Services.AuthApi.Service.Iservice;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mango.Services.AuthApi.Service.Implementation
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        //!important
        //we configure jwtoptions in program.cs 
        //because of this techniqe of configurtation we need to define IOptions<JwtOptions>
        //and pass options.value to dependency injection

        private readonly JwtOptions _jwtOptions;
        public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }
        //todo comment things here
        public string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);

            var claimList = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id),
                new Claim(JwtRegisteredClaimNames.Name, applicationUser.UserName)
            };

            //add roles
            claimList.AddRange(
                roles.Select(
                    role => new Claim(ClaimTypes.Role, role)));

            //todo comment for SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //this is the basic credits for jwt token
                Audience=_jwtOptions.Audience,
                Issuer=_jwtOptions.Issuer,
                Subject=new ClaimsIdentity(claimList),
                Expires=DateTime.UtcNow.AddDays(7),
                SigningCredentials=new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
                
            };
            var token=tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
