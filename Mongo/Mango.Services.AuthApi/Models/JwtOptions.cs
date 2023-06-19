namespace Mango.Services.AuthApi.Models
{
    //this is one way to take jwt settings, and save it to a class
    //else in program we should get the values from app settings
    public class JwtOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public string Secret { get; set; }=string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Issuer { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Audience { get; set; } = string.Empty;
    }
}
