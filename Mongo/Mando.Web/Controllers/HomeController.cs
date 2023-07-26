using IdentityModel;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mando.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _ProductService;
        private readonly ICardService _cardService;
        public HomeController(IProductService ProductService, ICardService cardService)
        {
            _ProductService = ProductService;
            _cardService = cardService;
        }

        public async Task<IActionResult>  Index()
        {
            List<ProductDto>? list = new();

            ResponseDto? responseDto = await _ProductService.GetAllProductAsync();

            if (responseDto != null && responseDto.IsSuccess == true)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(responseDto.Result));
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return View(list);
        }
        [Authorize]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            ProductDto? productDto = new();

            ResponseDto? responseDto = await _ProductService.GetProductByIdAsync(productId);

            if (responseDto != null && responseDto.IsSuccess == true)
            {
                productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(responseDto.Result));
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return View(productDto);
        }

        [Authorize]
        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDto productDto)
        {
            CartDto cartDto = new CartDto()
            {
                CartHeader =new CartHeaderDto
                {
                    UserId=User.Claims.Where(u=>u.Type==JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
                }
            };
            CartDetailsDto cartDetails = new CartDetailsDto()
            {
                Count=productDto.Count,
                ProductId=productDto.ProductId
            };

            List<CartDetailsDto> cartDetailsDtos = new() { cartDetails};
            cartDto.CartDetails = cartDetailsDtos;

            ResponseDto? responseDto = await _cardService.UpsertCardAsync(cartDto);

            if (responseDto != null && responseDto.IsSuccess == true)
            {
                TempData["success"] = "Item has been added to Shopping Card";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return View(productDto);
        }
        [Authorize(Roles =SD.RoleAdmin)]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}