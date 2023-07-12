

using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class ProductDto
    {   /// <summary>
        /// 
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// 
        /// </summary>
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
        public string CategoryName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Range(1, 100)]
        public int Count { get; set; } = 1;
    }
}
