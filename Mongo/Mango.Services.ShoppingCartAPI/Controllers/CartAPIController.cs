using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly ResponseDto _responseDto;
        private IProductService _productService;
        private ICouponService _couponService;
        private IMapper _mapper;
        private readonly IMessageBus _messageBus;
        private IConfiguration _config;
        public CartAPIController(AppDbContext appDbContext, IMapper mapper, IProductService productService,
            ICouponService couponService, IMessageBus messageBus, IConfiguration config)
        {
            _appDbContext = appDbContext;
            _productService = productService;
            _couponService = couponService;
            _responseDto = new ResponseDto();
            _mapper = mapper;
            _couponService = couponService;
            _messageBus = messageBus;
            _config = config;
        }
        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = new()
                {
                    CartHeader=_mapper.Map<CartHeaderDto>(
                        _appDbContext.CartHeaders.
                        First(u=>u.UserId==userId))
                };
                cartDto.CartDetails=_mapper.Map<IEnumerable<CartDetailsDto>>(
                    _appDbContext.CartDetails
                    .Where(u=>u.CardHeaderId==cartDto.CartHeader.CardHeaderId));

                IEnumerable<ProductDto> products = await _productService.GetProducts();
                foreach(var item in cartDto.CartDetails)
                {
                    item.Product = products.FirstOrDefault(u=>u.ProductId == item.ProductId);
                    cartDto.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }
                //apply coupon if any
                if(!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                {
                    CouponDto couponDto = await _couponService.GetCoupon(cartDto.CartHeader.CouponCode);

                    if(couponDto != null && cartDto.CartHeader.CartTotal > couponDto.MinAmount)
                    {
                        cartDto.CartHeader.CartTotal -= couponDto.DiscountAmount;
                        cartDto.CartHeader.Discount = couponDto.DiscountAmount; 
                    }
                }

                _responseDto.Result = cartDto;
            }
            catch 
            (Exception ex)
            {
                _responseDto.Message = ex.Message.ToString();
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cart)
        {
            try
            {
               var cardFromDb=await _appDbContext.CartHeaders
                    .FirstAsync(u=>u.UserId==cart.CartHeader.UserId);
                cardFromDb.CouponCode = cart.CartHeader.CouponCode;
                _appDbContext.CartHeaders.Update(cardFromDb);
                await _appDbContext.SaveChangesAsync();
                _responseDto.Result = true;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message.ToString();
                _responseDto.IsSuccess = false;
            }
            return _responseDto;

        }

        //[HttpPost("RemoveCoupon")]
        //public async Task<object> RemoveCoupon([FromBody] CartDto cart)
        //{
        //    try
        //    {
        //        var cardFromDb = await _appDbContext.CartHeaders
        //             .FirstAsync(u => u.UserId == cart.CartHeader.UserId);
        //        cardFromDb.CouponCode = "";
        //        _appDbContext.CartHeaders.Update(cardFromDb);
        //        await _appDbContext.SaveChangesAsync();
        //        _responseDto.Result = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _responseDto.Message = ex.Message.ToString();
        //        _responseDto.IsSuccess = false;
        //    }
        //    return _responseDto;

        //}

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cart)
        {
            try
            {
                var cartHeaderFromDb = await _appDbContext.CartHeaders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == cart.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //create header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cart.CartHeader);
                    _appDbContext.CartHeaders.Add(cartHeader);
                    await _appDbContext.SaveChangesAsync();
                    cart.CartDetails.First().CardHeaderId = cartHeader.CardHeaderId;
                    _appDbContext.CartDetails.Add(_mapper.Map<CartDetails>(cart.CartDetails.First()));
                    await _appDbContext.SaveChangesAsync();
                }
                else
                {
                    //check if details has same product
                    //AsNoTracking because we want to update the object and if we not set this
                    //entity core will confuse and give exception
                    var cartDetailsFromDb = await _appDbContext.CartDetails
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u =>
                        u.ProductId == cart.CartDetails.First().ProductId
                   && u.CardHeaderId == cartHeaderFromDb.CardHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        //create card details
                        cart.CartDetails.First().CardHeaderId = cartHeaderFromDb.CardHeaderId;
                        _appDbContext.CartDetails.Add(_mapper.Map<CartDetails>(cart.CartDetails.First()));
                        await _appDbContext.SaveChangesAsync();
                    }
                    else
                    {
                        //update count in cart details
                        cart.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cart.CartDetails.First().CardHeaderId = cartDetailsFromDb.CardHeaderId;
                        cart.CartDetails.First().CardDetailsId = cartDetailsFromDb.CardDetailsId;
                        _appDbContext.CartDetails.Update(_mapper.Map<CartDetails>(cart.CartDetails.First()));
                        await _appDbContext.SaveChangesAsync();
                    }
                }
                _responseDto.Result = cart;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message.ToString();
                _responseDto.IsSuccess = false;
            }
            return _responseDto;

        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody]int cardDetailsId)
        {
            try
            {
                var cartDetails= _appDbContext.CartDetails
                 .First(u => u.CardDetailsId == cardDetailsId);
               
                int totalItemOfCartDetail= _appDbContext.CartDetails
                 .Where(u => u.CardHeaderId == cartDetails.CardHeaderId).Count();
                
                _appDbContext.CartDetails.Remove(cartDetails);
                if (totalItemOfCartDetail == 1)
                {
                    var cartHeaderToRemove = await _appDbContext.CartHeaders
                 .FirstOrDefaultAsync(u => u.CardHeaderId == cartDetails.CardHeaderId);

                    _appDbContext.CartHeaders.Remove(cartHeaderToRemove);
                }
                
             
                _responseDto.Result = true;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message.ToString();
                _responseDto.IsSuccess = false;
            }
            return _responseDto;

        }

        [HttpPost("EmailCartRequest")]
        public async Task<object> EmailCartRequest([FromBody] CartDto cart)
        {
            try
            {
                await _messageBus.PublishMessage(cart,_config.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));
                _responseDto.Result = true;
            }
            catch (Exception ex)
            {
                _responseDto.Message = ex.Message.ToString();
                _responseDto.IsSuccess = false;
            }
            return _responseDto;

        }
    }
}
