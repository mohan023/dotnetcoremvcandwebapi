using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DemoDotnetCoreApplication.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;

namespace DemoDotnetCoreApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _congi;

        public HomeController(ILogger<HomeController> logger, IConfiguration congi)
        {
            _logger = logger;
            _logger.LogInformation("Home Contoller Executed");
            _logger.LogDebug("Home Controller Debug Message");
            _congi = congi;


        }

        public IActionResult Index()
        {
            try
            {
                ViewData["MyKey"] = _congi["MyKey"].ToString();
                ViewData["ConnectionString"] = "HardCoded";
                if (System.IO.File.Exists("/empty-dir-demo/testfile.txt"))
                {
                    // This path is a file
                    ViewData["FileContent"] = System.IO.File.ReadAllText("/empty-dir-demo/testfile.txt");
                }
                else
                {
                    ViewData["FileContent"] = "File Not Found";
                }
                Task<string> v = CallAPI();
                ViewData["apiresponse"] = v.Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

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

        public async Task<string> CallAPI()
        {
            using (var httpClient = new HttpClient())
            {
                var host = _congi["ApiHost"];
                _logger.LogTrace($"ApiHost value- {host}");
                
                using (var response = await httpClient.GetAsync($"{host}/WeatherForecast"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    //deserialize json to .net object
                    var reservationList = JsonConvert.DeserializeObject<List<WeatherForecast>>(apiResponse);
                    return string.Join(",", reservationList.Select(x => x.Summary));

                }

                
            }
        }
    }
}
