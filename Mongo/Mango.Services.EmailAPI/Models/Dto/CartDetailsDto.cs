namespace Mango.Services.EmailAPI.Models.Dto
{
    public class CartDetailsDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int CardDetailsId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CardHeaderId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public CartHeaderDto? CartHeader { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ProductDto? Product { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; }
    }
}
