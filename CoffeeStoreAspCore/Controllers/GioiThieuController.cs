using CoffeeStoreAspCore.Helper;
using CoffeeStoreAspCore.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeStoreAspCore.Controllers
{
    public class GioiThieuController: Controller
    {
        public IActionResult Index()
        {
            TempData["trang"] = "GioiThieu";

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
            return View();
        }
    }
}
