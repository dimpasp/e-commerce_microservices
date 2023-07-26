using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ShoppingCartAPI.Models
{
    public class CartDetails
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        [Key]
        public int CardDetailsId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CardHeaderId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [ForeignKey("CardHeaderId")]
        public CartHeader CartHeader { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public ProductDto Product { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; }
        
    }
}
