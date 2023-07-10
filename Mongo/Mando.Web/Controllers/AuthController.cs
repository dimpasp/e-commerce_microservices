using Mango.Services.AuthApi.Models;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;
        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }
        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new LoginRequestDto();
            return View(loginRequestDto);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto login)
        {
            ResponseDto response = await _authService.LoginAsync(login);
         

            if (response != null && response.IsSuccess)
            {
                LoginResponseDto loginResponseDto =
                     JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));
              
                await SignInUser(loginResponseDto);
               
                _tokenProvider.SetToken(loginResponseDto.Token);
                
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = response.Message;
                return View(login);
            }
        }
        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer}
            };
            //to pass list to view use viewbag
            ViewBag.RoleList = roleList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterationRequestDto registerationRequestDto)
        {
         ResponseDto response = await _authService.RegisterAsync(registerationRequestDto);
        ResponseDto asyncRole;

            if(response != null && response.IsSuccess)
            {
                if (string.IsNullOrWhiteSpace(registerationRequestDto.Role))
                {
                    registerationRequestDto.Role=SD.RoleAdmin;
                }
                asyncRole = await _authService.AssignRoleAsync(registerationRequestDto);
                if(asyncRole != null && asyncRole.IsSuccess)
                {
                    TempData["Success"] = "Registration Successfully!";
                    return View(RedirectToAction(nameof(Login)));
                }
            }
            else
            {
                TempData["error"] = response.Message;
            }
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer}
            };
            ViewBag.RoleList = roleList;
            return View(registerationRequestDto);
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();

            return RedirectToAction("Index","Home");
        }
            

      //todo add comments
        public async Task SignInUser(LoginResponseDto loginResponseDto)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(loginResponseDto.Token);

            var identity = new ClaimsIdentity(
                CookieAuthenticationDefaults.AuthenticationScheme);
          
            AddClaimsToIdentity(jwt, identity);

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        }

        private static void AddClaimsToIdentity(JwtSecurityToken jwt, ClaimsIdentity identity)
        {
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwt.Claims.FirstOrDefault(
                            x => x.Type == JwtRegisteredClaimNames.Email).Value));

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwt.Claims.FirstOrDefault(
                x => x.Type == JwtRegisteredClaimNames.Sub).Value));

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, jwt.Claims.FirstOrDefault(
                x => x.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(
               x => x.Type == JwtRegisteredClaimNames.Email).Value));

            identity.AddClaim(new Claim(ClaimTypes.Role, 
                jwt.Claims.FirstOrDefault(x => x.Type == "role").Value));
        }
    }
}
