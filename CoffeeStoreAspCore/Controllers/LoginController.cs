using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoffeeStoreAspCore.AdminApp.Service;
using CoffeeStoreAspCore.Data.EF;
using CoffeeStoreAspCore.Helper;
using CoffeeStoreAspCore.Models;
using CoffeeStoreAspCore.Service;
using CoffeeStoreAspCore.ViewModels.System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CoffeeStoreAspCore.Controllers
{
    public class LoginController : Controller
    {

        private readonly IUserApi _userApiClient;
        private readonly IConfiguration _configuration;

        public LoginController(IUserApi userApi, IConfiguration configuration)
        {
            _userApiClient = userApi;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (cart != null)
            {
                ViewBag.count = 0;
                foreach (var item in cart)
                {
                    ViewBag.count += item.Quantity;

                }
                ViewData["count"] = ViewBag.count;
            }
            ViewBag.a = SessionHelper.GetObjectFromJson<UserViewModel>(HttpContext.Session, "user");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DangNhap()
        {
            await HttpContext.SignOutAsync("esvlogin");
            return View();
        }

    


        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginRequest request)
        {
            StoreDBContext db = new StoreDBContext();
            if (!ModelState.IsValid)
                return View(ModelState);
            
            var token = await _userApiClient.Authenticate(request);
            /*   var identity = new ClaimsIdentity(new[] {
                       new Claim(ClaimTypes.Name, request.UserName),
                       new Claim(ClaimTypes.Role, "admin")
                   }, CookieAuthenticationDefaults.AuthenticationScheme);
                   var principal = new ClaimsPrincipal(identity);*/
            var userPrincipal = this.ValidateToken(token);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = false
            };
            await HttpContext.SignInAsync(
                            "esvlogin",
                            userPrincipal, authProperties);
           
            ViewBag.trang= TempData["trang"];
            return RedirectToAction("Index", ViewBag.trang);
           
        }

        [HttpPost]
        public async Task<IActionResult> DangXuat()
        {
            ViewBag.trang = TempData["trang"];

            await HttpContext.SignOutAsync("esvlogin");
            return RedirectToAction("Index", ViewBag.trang);
        }

        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;
            
            validationParameters.ValidAudience = _configuration["Tokens:Issuer"];
            validationParameters.ValidIssuer = _configuration["Tokens:Issuer"];
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }
    }
}