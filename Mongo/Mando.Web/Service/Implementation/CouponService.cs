using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service.Implementation
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;
        public CouponService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Data=couponDto,
                //todo after finish the course make it into appsettings,
                //bad techniqe to write 
                Url = SD.CouponAPIBase + "/api/coupon/"
            });
        }

        public async Task<ResponseDto?> DeleteCouponAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.DELETE,
                //todo after finish the course make it into appsettings,
                //bad techniqe to write 
                Url = SD.CouponAPIBase + "/api/coupon/" + id
            });
        }

        public async Task<ResponseDto?> GetAllCouponAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType=SD.ApiType.GET,
                //todo after finish the course make it into appsettings,
                //bad techniqe to write 
                Url=SD.CouponAPIBase+"/api/coupon"
            });
        }

        public async Task<ResponseDto?> GetCouponAsync(string couponCode)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.GET,
                //todo after finish the course make it into appsettings,
                //bad techniqe to write 
                Url = SD.CouponAPIBase + "/api/coupon/GetByCode/"+couponCode
            });
        }

        public async Task<ResponseDto?> GetCouponByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.GET,
                //todo after finish the course make it into appsettings,
                //bad techniqe to write 
                Url = SD.CouponAPIBase + "/api/coupon/" + id
            });
        }

        public async Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.PUT,
                Data = couponDto,
                //todo after finish the course make it into appsettings,
                //bad techniqe to write 
                Url = SD.CouponAPIBase + "/api/coupon/"
            });
        }
    }
}
