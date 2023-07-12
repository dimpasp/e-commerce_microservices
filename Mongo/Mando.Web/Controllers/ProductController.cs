using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _ProductService;
        public ProductController(IProductService ProductService)
        {
            _ProductService = ProductService;
        }
        //todo clear controller make it in an implementation class
        public async Task<IActionResult> ProductIndex()
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
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public async Task<IActionResult> ProductCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDto ProductDto)
        {
            //validate model in server side
            if (ModelState.IsValid)
            {
                ResponseDto? responseDto = await _ProductService.CreateProductAsync(ProductDto);

                if (responseDto != null && responseDto.IsSuccess == true)
                {
                    TempData["success"] = "Product created successfully!";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = responseDto?.Message;
                }
            }
            return View(ProductDto);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ProductDelete(int id)
        {
            ResponseDto? responseDto = await _ProductService.GetProductByIdAsync(id);

            if (responseDto != null && responseDto.IsSuccess == true)
            {
                ProductDto? ProductDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(responseDto.Result));
                return View(ProductDto);
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDto ProductDto)
        {
            //step 1 show the Product details
            ResponseDto? responseDto = await _ProductService.DeleteProductAsync(ProductDto.ProductId);

            if (responseDto != null && responseDto.IsSuccess == true)
            {
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return View(ProductDto);
        }
        public async Task<IActionResult> ProductEdit(int id)
        {
            ResponseDto? responseDto = await _ProductService.GetProductByIdAsync(id);

            if (responseDto != null && responseDto.IsSuccess == true)
            {
                ProductDto? ProductDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(responseDto.Result));
                return View(ProductDto);
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductDto ProductDto)
        {
            //step 1 show the Product details
            ResponseDto? responseDto = await _ProductService.UpdateProductAsync(ProductDto);

            if (responseDto != null && responseDto.IsSuccess == true)
            {
                TempData["success"] = "Product updated successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = responseDto?.Message;
            }
            return View(ProductDto);
        }
    }
}
