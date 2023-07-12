using System.ComponentModel.DataAnnotations;

namespace Mango.ProductApi.Models
{
    public class Product
    {
        /// <summary>
        /// 
        /// </summary>
        [Key]
        public int ProductId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Range(0, 1000)]
        public double Price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CategoryName { get; set;}
    }
}
