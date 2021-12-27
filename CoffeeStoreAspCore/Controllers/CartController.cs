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
    
    public class CartController : Controller
    {
        
        public async Task<IActionResult> IndexAsync()
        {
            TempData["trang"] = "DatHang";


            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:5001/api/Users/getbyname/" + User.Identity.Name))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<UserViewModel>(apiResponse);



                    var a = SessionHelper.GetObjectFromJson<List<KM>>(HttpContext.Session, "KM");
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
                        ViewBag.cart = cart;
                        ViewBag.Loi = "";
                        if (a != null)
                        {
                            ViewBag.km = a[0].CodeText;
                            if (a[0].TimesOfUsed == a[0].AvailableTimes)
                            {
                                ViewBag.Vocher = "Vocher đã hết hạn";
                                ViewBag.total = cart.Sum(item => item.Drink.UnitPrice * item.Quantity);

                            }
                            else
                            {
                                if (a[0].DiscountAmount == null)
                                {

                                    ViewBag.Loi = TempData["Loi"];
                                    ViewBag.total = cart.Sum(item => item.Drink.UnitPrice * item.Quantity) - ((cart.Sum(item => item.Drink.UnitPrice * item.Quantity) * a[0].DiscountPercent) / 100);

                                }
                                else
                                {


                                    ViewBag.Loi = TempData["Loi"];
                                    ViewBag.total = cart.Sum(item => item.Drink.UnitPrice * item.Quantity) - Convert.ToInt32(a[0].DiscountAmount);

                                }

                            }

                        }
                        else
                        {


                            ViewBag.Loi = TempData["Loi"];
                            ViewBag.total = cart.Sum(item => item.Drink.UnitPrice * item.Quantity);

                        }
                        return View(user);
                    }
                    else
                    {

                        ViewBag.cart = null;
                        ViewBag.err = "Không có gì trong giõ hàng";
                        return View(user);
                    }
                }
            }
        }
        public ActionResult GiohangPartial()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
           
            return PartialView();
        }

        public async Task<IActionResult> BuyAsync(int id)

        {
            StoreDBContext productModel = new StoreDBContext();
            Drink List = new Drink();

            using (var httpClient = new HttpClient())
            {
                using (
                    var response = await httpClient
                    .GetAsync("https://localhost:5001/api/Drink/GetById/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    List = JsonConvert.DeserializeObject<Drink>(apiResponse);
                }

            }
            if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") == null)
            {
                List<Item> cart = new List<Item>();
             
                cart.Add(new Item { Drink = productModel.Drinks.Find(id), Quantity = 1,ImagePath = List.DrinkImage.ImagePath });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    cart.Add(new Item { Drink = productModel.Drinks.Find(id), Quantity = 1,ImagePath = List.DrinkImage.ImagePath });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Index","Menu");
        }

        [Route("remove/{id}")]
        public IActionResult Remove(int id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = isExist(id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }

        [Route("edit1/{id}")]
        public IActionResult Edit1(int id)
        {
            
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = isExist(id);
            cart[index].Quantity++;
            
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }

        [Route("edit2/{id}")]
        public IActionResult Edit2(int id)
        {

            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = isExist(id);
            cart[index].Quantity--;
            if (cart[index].Quantity == 0)
            {
                cart.RemoveAt(index);
            }
            
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }

        private int isExist(int id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Drink.Id.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }

      

 
        public IActionResult Cancel()
        {
            
            SessionHelper.SetObjectAsJson(HttpContext.Session, "KM", null);
       
            TempData["Loi"] = "Cancel Thành Công";
            return RedirectToAction("Index", "Cart");
        }
        public async Task<IActionResult> KhuyenMaiAsync(string maKM)
        {
           KM List = new KM();
         
           using (var httpClient = new HttpClient())
           {
                    using (
                        var response = await httpClient
                        .GetAsync("https://localhost:5001/api/Voucher/GetByCode/" + maKM))

                    {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        List = JsonConvert.DeserializeObject<KM>(apiResponse);
                       
                        if (SessionHelper.GetObjectFromJson<List<KM>>(HttpContext.Session, "KM") == null)
                        {   
                            List<KM> kM = new List<KM>();

                            kM.Add(List);
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "KM", kM);
                          
                            TempData["Loi"] = "Nhập Code Thành Công";
                            return RedirectToAction("Index", "Cart");

                        }
                        else
                        {
                            List<KM> km = SessionHelper.GetObjectFromJson<List<KM>>(HttpContext.Session, "KM");

                            SessionHelper.SetObjectAsJson(HttpContext.Session, "KM", km);
                          
                            TempData["Loi"] = "Chỉ được áp dụng duy nhất 1 voucher cho 1 hóa đơn";
                            return RedirectToAction("Index", "Cart");
                            
                        }
                      
                       
                    }
                    else
                    {
                        TempData["Loi"] = "Không tồn tại mã code";
                        return RedirectToAction("Index", "Cart");

                    }


                }
                    
           }
            
       
            
        }
    }
}