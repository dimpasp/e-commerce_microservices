using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;
        public ProductService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> CreateProductAsync(ProductDto ProductDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Data=ProductDto,
                //todo after finish the course make it into appsettings,
                //bad techniqe to write 
                Url = SD.ProductAPIBase + "/api/Product/"
            });
        }

        public async Task<ResponseDto?> DeleteProductAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.DELETE,
                //todo after finish the course make it into appsettings,
                //bad techniqe to write 
                Url = SD.ProductAPIBase + "/api/Product/" + id
            });
        }

        public async Task<ResponseDto?> GetAllProductAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType=SD.ApiType.GET,
                //todo after finish the course make it into appsettings,
                //bad techniqe to write 
                Url=SD.ProductAPIBase+"/api/Product"
            });
        }

        public async Task<ResponseDto?> GetProductAsync(string ProductCode)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.GET,
                //todo after finish the course make it into appsettings,
                //bad techniqe to write 
                Url = SD.ProductAPIBase + "/api/Product/GetByCode/"+ProductCode
            });
        }

        public async Task<ResponseDto?> GetProductByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.GET,
                //todo after finish the course make it into appsettings,
                //bad techniqe to write 
                Url = SD.ProductAPIBase + "/api/Product/" + id
            });
        }

        public async Task<ResponseDto?> UpdateProductAsync(ProductDto ProductDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.PUT,
                Data = ProductDto,
                //todo after finish the course make it into appsettings,
                //bad techniqe to write 
                Url = SD.ProductAPIBase + "/api/Product/"
            });
        }
    }
}
