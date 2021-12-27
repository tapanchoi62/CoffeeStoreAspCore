using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoffeeStoreAspCore.Data.EF;
using CoffeeStoreAspCore.Helper;
using CoffeeStoreAspCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Identity;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using CoffeeStoreAspCore.ViewModels.Catalog.Vouchers;
using Microsoft.EntityFrameworkCore;
using CoffeeStoreAspCore.ViewModels.Catalog.BillDeatils;
using CoffeeStoreAspCore.ViewModels.Catalog.Bills;
using CoffeeStoreAspCore.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using CoffeeStoreAspCore.ViewModels.System;
namespace CoffeeStoreAspCore.Controllers
{
    [Authorize]
  
    public class DatHangController : Controller
    {
        [Authorize]
        [Route("dathang")]
        public async Task<IActionResult> IndexAsync()
        {
            TempData["trang"] = "DatHang";
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");

            ViewBag.sucsess = TempData["sucsess"];
            if (cart != null)
            {
                ViewBag.count = 0;
                foreach (var item in cart)
                {
                    ViewBag.count += item.Quantity;

                }
                ViewData["count"] = ViewBag.count;
            }
                using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:5001/api/Users/getbyname/" + User.Identity.Name))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<UserViewModel>(apiResponse);
                    return View(user);
                }
            }

        }

        [Authorize]
        public async Task<IActionResult> ThanhToan(UserViewModel login, UserUpdate userUpdate)
        {
            
            StoreDBContext db = new StoreDBContext();
            var b = SessionHelper.GetObjectFromJson<UserViewModel>(HttpContext.Session, "user");
            UserViewModel user = new UserViewModel();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:5001/api/Users/getbyname/" + User.Identity.Name))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    user = JsonConvert.DeserializeObject<UserViewModel>(apiResponse);
                }

            }

            var a = SessionHelper.GetObjectFromJson<List<KM>>(HttpContext.Session, "KM");
            var billViewModel = new BillCreateRequest();
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            var details = new List<BillDetailCreateRequest>();
            int i = 0;
            int total = 0;
            int totalActual = 0;
           
            if (cart != null)
            {


                foreach (var item in cart)
                {
                    details.Add(new BillDetailCreateRequest()
                    {
                        IdDrink = item.Drink.Id,
                        Quantity = item.Quantity
                    });

                    i += item.Quantity;
                    total += item.Quantity * item.Drink.UnitPrice;
                }
            }
          
            else
            {
                TempData["sucsess"] = "false";
                return RedirectToAction("Index", "Cart");
            }

            DateTime days = DateTime.Now;
            if (a != null)
            {
              
                    var maKm = a[0].CodeText;
                    VoucherViewModel km;
                    using (var httpClient = new HttpClient())
                    {
                        using (var response = await httpClient.GetAsync("https://localhost:5001/api/Voucher/GetByCode/" + maKm))
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            km = JsonConvert.DeserializeObject<VoucherViewModel>(apiResponse);
                        }

                    }
                    totalActual = total;
                    if (km.DiscountAmount != null)
                        totalActual = total - Convert.ToInt32(km.DiscountAmount);
                    if (km.DiscountPercent != null)
                        totalActual = total - Convert.ToInt32(km.DiscountPercent) * total / 100;

                
                billViewModel = new BillCreateRequest()
                {
                    Days = days,
                    TotalQuantity = i,
                    ActualTotalAmount = totalActual,
                    CalculatedTotalAmount = total,
                    IdUser = user.Id,
                    IdVoucher = a[0].Id,
                    BillDetails = details
                };
            }
            else
            {
                billViewModel = new BillCreateRequest()
                {
                    Days = days,
                    TotalQuantity = i,
                    ActualTotalAmount = total,
                    CalculatedTotalAmount = total,
                    IdUser = user.Id,
                    //IdVoucher = a[0].Id,


                    BillDetails = details
                };
            }

            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(billViewModel);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:5001/api/Order/CreateNew", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var ttkh = db.Users.First(a => a.Id == user.Id);
                    ttkh.Address = userUpdate.Address;
                    ttkh.PhoneNumber = userUpdate.PhoneNumber;
                    db.SaveChanges();

                    TempData["sucsess"] = "true";
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", null);
                    if (billViewModel.IdVoucher != null)
                    {
                        var update = db.Vouchers.First(a => a.Id == billViewModel.IdVoucher);
                        update.AvailableTimes++;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index", "Cart");

                }
                else
                {
                    TempData["sucsess"] = "false";
                    return RedirectToAction("Index", "Cart");
                }

            }

        }

    }

}