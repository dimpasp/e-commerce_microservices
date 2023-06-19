using Mango.Services.AuthApi.Data;
using Mango.Services.AuthApi.Models;
using Mango.Services.AuthApi.Models.Dto;
using Mango.Services.AuthApi.Service.Iservice;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthApi.Service.Implementation
{
    public class AuthService : IAuthService
    {
        //UserManager and RoleManager are libraries given from Microsoft.AspNetCore.Identity
        //the help to manage user
        //in UserManager we set the class we create and not the default identity user
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext appDbContext, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;

        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _appDbContext.ApplicationUsers.FirstOrDefault(
                 x => x.UserName.ToLower() == email.ToLower());
            if (user != null)
            {
                //first check if role exist 
                //this method is given from Microsoft.AspNetCore.Identity
                //todo important
                //_roleManager.RoleExistsAsync(roleName) =>this not work because it is asychronous method
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user=_appDbContext.ApplicationUsers.FirstOrDefault(
                x=>x.UserName.ToLower()==loginRequestDto.UserName.ToLower());
            
            //check password,for login we need combination of user and password
            bool isValid= await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
           
            if(user==null || isValid==false)
            {
                return new LoginResponseDto()
                {
                    User=null,
                    Token=string.Empty
                };
            }
            //if user was found generate token

            //first get user obj
            UserDto userLogin = new()
            {
                Email = user.Email,
                Name = user.Name,
                ID = user.Id,
                PhoneNumber = user.PhoneNumber
            };

            //get token
            var token= _jwtTokenGenerator.GenerateToken(user);

            LoginResponseDto loginResponseDto = new()
            {
                User = userLogin,
                Token = string.Empty
            };

            return loginResponseDto;
        }

        public async Task<string> Reqister(RegisterationRequestDto registerationRequestDto)
        {
            ApplicationUser applicationUser = new()
            {
                UserName = registerationRequestDto.Email,//?
                Email = registerationRequestDto.Email,
                NormalizedEmail = registerationRequestDto.Email.ToUpper(),
                Name = registerationRequestDto.Name,
                PhoneNumber = registerationRequestDto.PhoneNumber
            };
            try
            {
                var result = await _userManager.CreateAsync(applicationUser, registerationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToRetutn = _appDbContext.ApplicationUsers.First(
                        x => x.UserName == registerationRequestDto.Email);

                    UserDto user = new()
                    {
                        Email = userToRetutn.Email,
                        Name = userToRetutn.Name,
                        ID = userToRetutn.Id,
                        PhoneNumber = userToRetutn.PhoneNumber
                    };
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
    }
}
