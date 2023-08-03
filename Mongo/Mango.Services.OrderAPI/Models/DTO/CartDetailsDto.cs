namespace Mango.Services.OrderAPI.Models.DTO
{
    public class CartDetailsDto
    {
        public int CardDetailsId { get; set; }
        public int CardHeaderId { get; set; }
        public CartHeaderDto? CartHeader { get; set; }
        public int ProductId { get; set; }
        public ProductDto? Product { get; set; }
        public int Count { get; set; }
    }
}
