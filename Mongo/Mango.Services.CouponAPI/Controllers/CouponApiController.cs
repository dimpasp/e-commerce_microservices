using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.Xml;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponApiController : ControllerBase
    {
        //dependency injection
        private readonly AppDbContext _appDbContext;
        private readonly ResponseDto _responseDto;
        private IMapper _mapper;
        public CouponApiController(AppDbContext appDbContext, IMapper mapper)
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
                //todo after video if we want IQueryable
                IEnumerable<Coupon> coupons = _appDbContext.Coupons.ToList();
                //_responseDto.Result= coupons;
                _responseDto.Result = _mapper.Map<IEnumerable<CouponDto>>(coupons);
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
        //this is how we set that we want integer in url
        public ResponseDto Get(int id)
        {
            try
            {
                Coupon coupons = _appDbContext.Coupons.First(x => x.CouponId == id);
                //use automapper 
                _responseDto.Result = _mapper.Map<CouponDto>(coupons);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto Get(string code)
        {
            try
            {
                Coupon coupons = _appDbContext.Coupons.FirstOrDefault(x => x.CouponCode.ToLower() == code.ToLower());
                if (coupons == null)
                    _responseDto.IsSuccess = false;

                _responseDto.Result = _mapper.Map<CouponDto>(coupons);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpPost]
        public ResponseDto Post([FromBody]CouponDto couponDto)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDto);
                
                _appDbContext.Coupons.Add(coupon);
                _appDbContext.SaveChanges();
                 
                _responseDto.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }
        [HttpPut]
        public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDto);

                _appDbContext.Coupons.Update(coupon);
                _appDbContext.SaveChanges();

                _responseDto.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpDelete]
        public ResponseDto Delete(int id)
        {
            try
            {
                Coupon coupon = _appDbContext.Coupons.First(x=>x.CouponId == id);

                _appDbContext.Coupons.Remove(coupon);
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
