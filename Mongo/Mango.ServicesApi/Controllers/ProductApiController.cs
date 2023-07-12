using AutoMapper;
using Mango.ProductApi.Data;
using Mango.ProductApi.Models;
using Mango.ProductApi.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.ProductApi.Controllers
{
    [Route("api/product")]
    [ApiController]
    //[Authorize]
    public class ProductApiController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly ResponseDto _responseDto;
        private IMapper _mapper;
        public ProductApiController(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _responseDto = new ResponseDto();
            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Product> Products = _appDbContext.Products.ToList();
                _responseDto.Result = _mapper.Map<IEnumerable<ProductDto>>(Products);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Product Products = _appDbContext.Products.First(x => x.ProductId == id);
                _responseDto.Result = _mapper.Map<ProductDto>(Products);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpPost]
        [Authorize(Roles ="ADMIN")]
        public ResponseDto Post([FromBody]ProductDto ProductDto)
        {
            try
            {
                Product Product = _mapper.Map<Product>(ProductDto);
                
                _appDbContext.Products.Add(Product);
                _appDbContext.SaveChanges();
                 
                _responseDto.Result = _mapper.Map<ProductDto>(Product);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }
        [HttpPut]
       [Authorize(Roles = "ADMIN")]
        public ResponseDto Put([FromBody] ProductDto ProductDto)
        {
            try
            {
                Product Product = _mapper.Map<Product>(ProductDto);

                _appDbContext.Products.Update(Product);
                _appDbContext.SaveChanges();

                _responseDto.Result = _mapper.Map<ProductDto>(Product);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        //we want to pass a parameter so we have not forget to add route
        public ResponseDto Delete(int id)
        {
            try
            {
                Product Product = _appDbContext.Products.First(x=>x.ProductId == id);

                _appDbContext.Products.Remove(Product);
                _appDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }
    }
}
