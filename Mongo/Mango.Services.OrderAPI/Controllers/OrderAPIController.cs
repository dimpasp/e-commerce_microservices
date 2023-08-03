using AutoMapper;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.DTO;
using Mango.Services.OrderAPI.Service.IService;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly ResponseDto _responseDto;
        private IProductService _productService;
        private IMapper _mapper;

        public OrderAPIController(AppDbContext appDbContext, IMapper mapper, IProductService productService)
        {
            _appDbContext = appDbContext;
            _productService = productService;
            _responseDto = new ResponseDto();
            _mapper = mapper;

        }
        //  [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.OrderDetails =
                    _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);

                OrderHeader orderCreated =
                     _appDbContext.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;

                await _appDbContext.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                _responseDto.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {
                _responseDto.Result = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        //  [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            

            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApproveUrl,
                    CancelUrl=stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment"
                };

                var discountsObj = new List<SessionDiscountOptions>(){
                    new SessionDiscountOptions
                    {
                        Coupon=stripeRequestDto.OrderHeader.CouponCode
                    }
                };
                //because it is a list of items and i want to send them all
                foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            //price of item
                            UnitAmount = (long)(item.Price * 100),//$20.99 => 2099
                            Currency="usd",//usa dollars
                            ProductData=new SessionLineItemPriceDataProductDataOptions
                            {
                                Name=item.ProductName
                                //here we can give more information about product details
                            }
                        },
                        Quantity=item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                //apply discount object 
                if (stripeRequestDto.OrderHeader.Discount > 0)
                {
                    options.Discounts = discountsObj;
                }

                var service = new SessionService();
                //save to local variable the session to retrieve information
                Session session= service.Create(options);

                stripeRequestDto.StripeSessionId = session.Url;
                OrderHeader orderHeader = _appDbContext.OrderHeaders.First(
                    u => u.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);
                orderHeader.StripsSessionId = session.Id;
                _appDbContext.SaveChanges();
                _responseDto.Result = stripeRequestDto;

            }
            catch (Exception ex)
            {
                _responseDto.Result = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        //  [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {


            try
            {
                OrderHeader orderHeader = _appDbContext.OrderHeaders.First(
                    u => u.OrderHeaderId == orderHeaderId);

                var service = new SessionService();
                //save to local variable the session to retrieve information
                Session session = service.Get(orderHeader.StripsSessionId);
                var paymentIntentServie = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentServie.Get(session.PaymentIntentId);


                if (paymentIntent.Status =="succeeded")
                {
                    //then payment is successfull
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = SD.Status_Approved;
                    _appDbContext.SaveChanges();

                    _responseDto.Result=_mapper.Map<OrderHeaderDto>(orderHeader);

                }
           

            }
            catch (Exception ex)
            {
                _responseDto.Result = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }
    }
}
