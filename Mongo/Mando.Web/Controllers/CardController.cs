using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CardController : Controller
    {
        private readonly ICardService _cardService;
        
        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartBasedOnLoggedInUser());
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims
               .Where(u => u.Type == JwtRegisteredClaimNames.Sub)?
               .FirstOrDefault()?.Value;

            var response = await _cardService.RemoveFromCardAsync(cartDetailsId);

            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {

            var response = await _cardService.ApplyCouponAsync(cartDto);

            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader.CouponCode = string.Empty;
            var response = await _cardService.ApplyCouponAsync(cartDto);

            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        private async Task<CartDto> LoadCartBasedOnLoggedInUser()
        {
            var userId = User.Claims
                .Where(u => u.Type == JwtRegisteredClaimNames.Sub)?
                .FirstOrDefault()?.Value;

            var response= await _cardService.GetCardByUserIdAsync(userId);
            
            if (response != null & response.IsSuccess)
            {
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
                return cartDto;
            }
            return new CartDto();
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {
            CartDto cart = await LoadCartBasedOnLoggedInUser();

            cart.CartHeader.Email = User.Claims
                .Where(u => u.Type == JwtRegisteredClaimNames.Email)?
                .FirstOrDefault()?.Value;

            var response = await _cardService.EmailCart(cart);

            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Email will be processed and sent shortly.";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
    }
}
