using CoffeeStoreAspCore.Helper;
using CoffeeStoreAspCore.Models;
using CoffeeStoreAspCore.ViewModels.System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeStoreAspCore.Controllers
{
    public class RegisterController : Controller
    {
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
            // ViewBag.a = SessionHelper.GetObjectFromJson<UserViewModel>(HttpContext.Session, "user");
            return View();
        }

        public async Task<IActionResult> DangKiAsync(RegisterRequest request)
        {
            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(request);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync
            ("https://localhost:5001/api/users/register", httpContent);

                return RedirectToAction("Index", "Login");
            }
            // ViewBag.a = SessionHelper.GetObjectFromJson<UserViewModel>(HttpContext.Session, "user");
            return View();
        }
    }
}
