using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CoffeeStoreAspCore.Data.EF;
using CoffeeStoreAspCore.Data.Entities;
using CoffeeStoreAspCore.Helper;
using CoffeeStoreAspCore.Models;
using CoffeeStoreAspCore.ViewModels.Catalog.Contacts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CoffeeStoreAspCore.Controllers
{
    public class FeedbackController : Controller
    {

        public IActionResult Index()
        {
            TempData["trang"] = "Feedback";

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
                ViewBag.err = TempData["Error"];
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> MessageAsync(string txtName, string txtEmail, string txtPhone, string txtMsg)
        {
            var contract = new Contact()
            {
                Email = txtEmail,
                Message = txtMsg,
                Name = txtName,
                PhoneNumber = txtPhone
            };
            ////StoreDBContext db = new StoreDBContext();
            ////db.Contacts.Add(contract);
            ////int test = db.SaveChanges();
            ////if(test == 1)
            ////{
            ////    TempData["Error"] = "Cảm ơn bạn đã góp ý cho chúng tôi";
            ////    return RedirectToAction("Index");
            ////}



            using HttpClient client = new HttpClient();
            var json = JsonConvert.SerializeObject(contract);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:5001/api/Contact/CreateNew", httpContent);
            if (response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Cảm ơn bạn đã góp ý cho chúng tôi";
            }
            return RedirectToAction("Index");
        }
    }
}