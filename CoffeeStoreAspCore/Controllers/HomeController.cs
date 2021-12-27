using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CoffeeStoreAspCore.Models;

using System.Net.Http;

using CoffeeStoreAspCore.ViewModels.Menu;
using CoffeeStoreAspCore.ViewModels.Catalog;
using Newtonsoft.Json;
using CoffeeStoreAspCore.ViewModels.System;
using CoffeeStoreAspCore.ViewModels.Catalog.Bills;
using CoffeeStoreAspCore.Data.EF;
using System.Net.WebSockets;
using CoffeeStoreAspCore.ViewModels.Catalog.BillOrders;
using CoffeeStoreAspCore.Helper;

namespace CoffeeStoreAspCore.Controllers
{
    public class HomeController : Controller
    {
        UserViewModel a = new UserViewModel();

        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
       
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            TempData["trang"] = "Home";
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
          
            var user = User.Identity.Name;
            List<MenuViewModel> LsMenu = new List<MenuViewModel>();
            List<DrinkViewModel> List = new List<DrinkViewModel>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:5001/api/Drink/GetTop"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    List = JsonConvert.DeserializeObject<List<DrinkViewModel>>(apiResponse);
                }

            }
            return View(List);


        }
        [HttpGet]
        public async Task<IActionResult> ThongTinDangNhap()
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
            ViewBag.save = TempData["save"];
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:5001/api/Users/getbyname/" + User.Identity.Name)) 
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    a = JsonConvert.DeserializeObject<UserViewModel>(apiResponse);
                }

            }
            
            return View(a);
          
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserUpdate request )
        {
            var id = request.Id;
           
                using (HttpClient client = new HttpClient())
                {
                    string stringData = JsonConvert.SerializeObject(request);
                    var contentData = new StringContent(stringData,
                System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync
                ("https://localhost:5001/api/Users/"+id,
                contentData);
                if(response.IsSuccessStatusCode)
                {
                    TempData["save"] = "true";
                }
                else
                    TempData["save"] = "false";
                ViewBag.Message = response.Content.
                ReadAsStringAsync().Result;


                    return RedirectToAction("ThongTinDangNhap");
                }
            

        }
        public async Task<IActionResult> ThongTinBill()
        {
            ViewBag.huy = TempData["huy"];
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
            List<BillViewModel> bill = new List<BillViewModel>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:5001/api/Users/getbyname/" + User.Identity.Name))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    a = JsonConvert.DeserializeObject<UserViewModel>(apiResponse);
                }

            }

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:5001/api/Order/GetByIdUser/" +a.Id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    bill = JsonConvert.DeserializeObject<List<BillViewModel>>(apiResponse);
                    
                }

            }




            return View(bill);

        }

        public async Task<IActionResult> Pending(int id, string Status)
        {
            using (HttpClient client = new HttpClient())
            {
                string stringData = JsonConvert.SerializeObject(Status);
                var contentData = new StringContent(stringData,
            System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PatchAsync
            ("https://localhost:5001/api/Order/ChangeStatus/" + id + "/" + Status,
            contentData);
                ViewBag.Message = response.Content.
            ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                    TempData["huy"] = "true";
                else
                    TempData["huy"] = "false";

                return RedirectToAction("ThongTinBill");
            }
        }
    }
}
