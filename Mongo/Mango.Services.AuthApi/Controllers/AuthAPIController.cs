using Mango.MessageBus;
using Mango.Services.AuthApi.Models.Dto;
using Mango.Services.AuthApi.Service.Iservice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Mango.Services.AuthApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private IAuthService _authService;
        private IMessageBus _messageBus;
        private IConfiguration _configuration;
        protected ResponseDto _responseDto;//comment why protected
        public AuthAPIController(IAuthService authService, IMessageBus messageBus, IConfiguration configuration)
        {
            _authService = authService;
            _messageBus = messageBus;
            _configuration = configuration;
            _responseDto = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDto registerationRequestDto)
        {
            //todo after finish the course to change this logic that return string and check this
            var result = await _authService.Reqister(registerationRequestDto);
            if(!result.IsNullOrEmpty())
            {
                _responseDto.Result = false;
                _responseDto.Message = result.ToString();
                return BadRequest(_responseDto);
            }
            //send message to db throw MessageBus
            await _messageBus.PublishMessage(registerationRequestDto.Email,_configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));

            return Ok(_responseDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var loginResponse = await _authService.Login(loginRequestDto);
            if(loginResponse.User == null)
            {
                _responseDto.Result = false;
                _responseDto.Message = "User or Password is not correct";
                return BadRequest(_responseDto);
            }
            //todo
            _responseDto.Result = loginResponse;
           return Ok(_responseDto);
        }
        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegisterationRequestDto registerationRequestDto)
        {
            var assignRole = await _authService.AssignRole(registerationRequestDto.Email,registerationRequestDto.Role.ToUpper());
            if (!assignRole)
            {
                _responseDto.Result = false;
                _responseDto.Message = "Error encountered";
                return BadRequest(_responseDto);
            } 
            //todo is this wrong?
            
            return Ok(_responseDto);
        }
    }
}
