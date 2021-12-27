using CoffeeStoreAspCore.ViewModels.Catalog.Vouchers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoffeeStoreAspCore.Controllers
{
    public class Khuyenmai : Controller
    {
        [Route("index")]
        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
         
            List<VoucherViewModel> List = new List<VoucherViewModel>();

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:5001/api/Voucher/GetAll"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    List = JsonConvert.DeserializeObject<List<VoucherViewModel>>(apiResponse);
                }

            }

      

            return View(List);
        }
    }
}
