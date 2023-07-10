using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class RegisterationRequestDto
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Email { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Role { get; set; }
    }
}
