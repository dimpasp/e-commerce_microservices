using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }
        //todo clear controller make it in an implementation class
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto>? list = new();

            ResponseDto? responseDto = await _couponService.GetAllCouponAsync();

            if (responseDto != null && responseDto.IsSuccess == true)
            {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(responseDto.Result));
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return View(list);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public async Task<IActionResult> CouponCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDto couponDto)
        {
            //validate model in server side
            if (ModelState.IsValid)
            {
                ResponseDto? responseDto = await _couponService.CreateCouponAsync(couponDto);

                if (responseDto != null && responseDto.IsSuccess == true)
                {
                    TempData["success"] = "Coupon created successfully!";
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = responseDto?.Message;
                }
            }
            return View(couponDto);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> CouponDelete(int id)
        {
            ResponseDto? responseDto = await _couponService.GetCouponByIdAsync(id);

            if (responseDto != null && responseDto.IsSuccess == true)
            {
                CouponDto? couponDto = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(responseDto.Result));
                return View(couponDto);
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDto couponDto)
        {
            //step 1 show the coupon details
            ResponseDto? responseDto = await _couponService.DeleteCouponAsync(couponDto.CouponId);

            if (responseDto != null && responseDto.IsSuccess == true)
            {
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return View(couponDto);
        }
    }
}
