using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoffeeStoreAspCore.ViewModels.Catalog.Report;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CoffeeStoreAspCore.AdminApp.Controllers
{
    public class ThongKeDoanhThuTheoNamController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<ReportYearView> ListYear = new List<ReportYearView>();
            using (var httpClient = new HttpClient())
            {
               
                using (var response = await httpClient.GetAsync("https://localhost:5001/api/Order/Report"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ListYear = JsonConvert.DeserializeObject<List<ReportYearView>>(apiResponse);
                }

            }
         
            return View(ListYear);
        }
    }
}