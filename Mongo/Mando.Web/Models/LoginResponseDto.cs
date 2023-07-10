namespace Mango.Web.Models
{
    public class LoginResponseDto
    {
        /// <summary>
        /// jwt token
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public UserDto User { get; set; }
    }
}
