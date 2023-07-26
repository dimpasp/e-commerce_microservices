using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Mango.Services.ShoppingCartAPI.Service.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
   
        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            //based on the name will examine in program.cs and get the address
            var client=_httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/product"); 
            var apiContent=await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>
                    (Convert.ToString(resp.Result));
            }
            else
                return new List<ProductDto>();
        }
    }
}
