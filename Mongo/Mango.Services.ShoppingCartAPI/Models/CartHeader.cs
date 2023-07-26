using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCartAPI.Models
{
    public class CartHeader
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
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
        /// we calculate it in runtime, we don't want to save it to database
        /// </summary>
        [NotMapped]       
        public double Discount { get; set; }
        /// <summary>
        /// we calculate it in runtime, we don't want to save it to database
        /// </summary>
        [NotMapped]
        public double CartTotal { get; set; }
    }
}
