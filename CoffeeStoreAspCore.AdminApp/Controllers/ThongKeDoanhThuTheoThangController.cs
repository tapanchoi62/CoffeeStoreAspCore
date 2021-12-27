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
    public class ThongKeDoanhThuTheoThangController : Controller
    {
        public async Task<IActionResult> Index(int year=0)
        {
            var now = DateTime.Now;
            var Y = year == 0 ? now.Year: year;
            ViewBag.year = Y;
            List<ReportMonthView> ListYear = new List<ReportMonthView>();
            using (var httpClient = new HttpClient())
            {

                using (var response = await httpClient.GetAsync("https://localhost:5001/api/Order/Report/"+Y))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ListYear = JsonConvert.DeserializeObject<List<ReportMonthView>>(apiResponse);
                }

            }

            return View(ListYear);
        }
    }
}
