namespace Mango.Services.EmailAPI.Models.Dto
{
    public class CartDto
    {
        /// <summary>
        /// 
        /// </summary>
        public CartHeaderDto CartHeader{ get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<CartDetailsDto>? CartDetails { get; set;}
    }
}
