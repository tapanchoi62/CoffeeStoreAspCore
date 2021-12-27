using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CoffeeStoreAspCore.AdminApp.Models;
using Microsoft.AspNetCore.Authorization;
using CoffeeStoreAspCore.ViewModels.Menu;
using System.Net.Http;
using Newtonsoft.Json;
using CoffeeStoreAspCore.ViewModels.Catalog;
using CoffeeStoreAspCore.ViewModels.Catalog.Contacts;
using CoffeeStoreAspCore.ViewModels.Catalog.Bills;

namespace CoffeeStoreAspCore.AdminApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
       [Authorize(Roles = "Admin")]
      
        public async Task<IActionResult> IndexAsync()
        {

            List<MenuViewModel> ListMenu = new List<MenuViewModel>();
            List<DrinkViewModel> ListDrink = new List<DrinkViewModel>();
            List<ContactViewModel> ListContact = new List<ContactViewModel>();
            List<BillViewModel> List = new List<BillViewModel>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:5001/api/Menu/GetAll"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ListMenu = JsonConvert.DeserializeObject<List<MenuViewModel>>(apiResponse);
                }
                using (var response = await httpClient.GetAsync("https://localhost:5001/api/Drink/GetAll"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ListDrink = JsonConvert.DeserializeObject<List<DrinkViewModel>>(apiResponse);
                }

                using (var response = await httpClient.GetAsync("https://localhost:5001/api/Contact/GetAll"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ListContact = JsonConvert.DeserializeObject<List<ContactViewModel>>(apiResponse);
                }
                using (var response = await httpClient.GetAsync("https://localhost:5001/api/Order/GetAllBillNew"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    List = JsonConvert.DeserializeObject<List<BillViewModel>>(apiResponse);
                }
    
            }
            ViewBag.Menu = ListMenu.Count();
            ViewBag.Drink = ListDrink.Count();
            ViewBag.Contact = ListContact.Count();
            ViewBag.Order = List.Count();
           var user = User.Identity.Name;
            return View();
        }

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
