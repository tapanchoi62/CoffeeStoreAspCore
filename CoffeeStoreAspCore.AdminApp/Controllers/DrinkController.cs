using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CoffeeStoreAspCore.ViewModels.Catalog;
using CoffeeStoreAspCore.ViewModels.Catalog.Drinks;
using CoffeeStoreAspCore.ViewModels.DrinkRepo;
using CoffeeStoreAspCore.ViewModels.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CoffeeStoreAspCore.AdminApp.Controllers
{
    [Authorize]
    public class DrinkController : Controller
    {

        [Authorize(Roles = "Admin")]
        [HttpGet]
  
        public async Task<IActionResult> Index()
        {

            List<MenuViewModel> LsMenu = new List<MenuViewModel>();
            List<DrinkViewModel> List = new List<DrinkViewModel>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:5001/api/Drink/GetAll"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    List = JsonConvert.DeserializeObject<List<DrinkViewModel>>(apiResponse);
                }

            }
            if (TempData["kq"] != null)
                ViewBag.mgs = TempData["kq"].ToString();
            return View(List);
        }
        [Authorize(Roles = "Admin")]
      [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<MenuViewModel> LsMenu = new List<MenuViewModel>();
            using (var httpClient = new HttpClient())
            {
                using (var rep = await httpClient.GetAsync("https://localhost:5001/api/Menu/GetAll"))
                {
                    string apiRep = await rep.Content.ReadAsStringAsync();
                    LsMenu = JsonConvert.DeserializeObject<List<MenuViewModel>>(apiRep);
                }
                ViewBag.listMenu = LsMenu;
            }

            return View("Create");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
      
        public async Task<ActionResult> Create(DrinkCreateRequest request)
        {
             string USER_CONTENT_FOLDER_NAME = "user-content";
            using (HttpClient client = new HttpClient())
                {
                var originalFileName = ContentDispositionHeaderValue.Parse(request.ThumbnailImage.ContentDisposition).FileName.Trim('"');
               /* var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
              var  _userContentFolder = Path.Combine("localhost:5001", USER_CONTENT_FOLDER_NAME);

                var filePath = Path.Combine(_userContentFolder, fileName);
                using var output = new FileStream(filePath, FileMode.Create);
               await request.ThumbnailImage.OpenReadStream().CopyToAsync(output);*/
                var drink = new DrinkRequest();
                drink.Name = request.Name;
                drink.UnitPrice = request.UnitPrice;
                drink.IdMenu = request.IdMenu;
                drink.fileName = originalFileName;

                var json = JsonConvert.SerializeObject(drink);
              
                var reponse = new StringContent(json, Encoding.UTF8, "application/json");
              
                var result = await client.PostAsync
                ("https://localhost:5001/api/Drink/CreateNew", reponse);

                if (result.IsSuccessStatusCode)
                    TempData["kq"] = "Thêm thành công";

                return RedirectToAction("Index");


                }

         
            

        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
     
        public async Task<ActionResult> Edit(int id)
        {
            List<MenuViewModel> LsMenu = new List<MenuViewModel>();

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response =
client.GetAsync("https://localhost:5001/api/Drink/GetById/" + id).Result;
                string stringData = response.Content.
            ReadAsStringAsync().Result;
                var data = JsonConvert.
            DeserializeObject<DrinkUpdateRequest>(stringData);
                using (var rep = await client.GetAsync("https://localhost:5001/api/Menu/GetAll"))
                {
                    string apiRep = await rep.Content.ReadAsStringAsync();
                    LsMenu = JsonConvert.DeserializeObject<List<MenuViewModel>>(apiRep);
                }
              
                ViewBag.listMenu = LsMenu;
                return View(data);

            }

        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Edit(DrinkUpdateRequest request)
        {
            using (HttpClient client = new HttpClient())
            {
                string stringData = JsonConvert.SerializeObject(request);
                var contentData = new StringContent(stringData,
            System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync
            ("https://localhost:5001/api/Drink/Update",
            contentData);
                ViewBag.Message = response.Content.
            ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                    TempData["kq"] = "Cập nhật thành công";

                return RedirectToAction("Index", "Drink");
            }
        }
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id, string Status)
        {
            using (HttpClient client = new HttpClient())
            {
                string stringData = JsonConvert.SerializeObject(Status);
                var contentData = new StringContent(stringData,
            System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PatchAsync
            ("https://localhost:5001/api/Drink/ChangeStatus/" + id + "/" + Status,
            contentData);
                ViewBag.Message = response.Content.
            ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                    TempData["kq"] = "Xóa thành công";


                return RedirectToAction("Index", "Drink");
            }
        }
    }
}