using Mango.Services.AuthApi.Models.Dto;

namespace Mango.Services.AuthApi.Service.Iservice
{
    public interface IAuthService
    {
        /// <summary>
        /// in register we create the user and his details
        /// or try this Task<UserDto>
        /// </summary>
        /// <param name="registerationRequestDto"></param>
        /// <returns></returns>
        Task<string> Reqister(RegisterationRequestDto registerationRequestDto);
        /// <summary>
        /// in login we get user and give him a token
        /// </summary>
        /// <param name="loginRequestDto"></param>
        /// <returns></returns>
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginRequestDto"></param>
        /// <returns></returns>
        Task<bool> AssignRole(string email,string roleName);
    }
}
