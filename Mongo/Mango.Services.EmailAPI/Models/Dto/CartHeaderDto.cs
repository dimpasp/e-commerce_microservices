namespace Mango.Services.EmailAPI.Models.Dto
{
    public class CartHeaderDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int CardHeaderId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? CouponCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double Discount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double CartTotal { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Phone { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Email { get; set; }
    }
}
