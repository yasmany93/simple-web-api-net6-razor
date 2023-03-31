using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication2.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using PagedList;
using DataAccessLayer;

namespace WebApplication2.Controllers
{
    public class ToLowerNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            return name.ToLower();
        }
    }

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string ACCESS_TOKEN = APIConectionInfo.ACCESS_TOKEN;
        private readonly string URL = APIConectionInfo.URL;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<UserDTO> usersList = new List<UserDTO>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{URL}?access-token={ACCESS_TOKEN}"))
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    usersList = JsonSerializer.Deserialize<List<UserDTO>>(apiResponse, options);
                }
            }

            return View(usersList);
        }

        public async Task<IActionResult> Create()
        {
            var user = new UserDTO
            {
                Name = "",
                Email = "",
                Gender = "",
                Status = ""
            };
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserDTO user)
        {

            using (var httpClient = new HttpClient())
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = new ToLowerNamingPolicy()
                };
                var json = JsonSerializer.Serialize<UserDTO>(user, options).ToString();
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await httpClient.PostAsync($"{URL}?access-token={ACCESS_TOKEN}", content);

            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            //if(id == null) 
            var user = new UserDTO
            {
                Id = 0,
                Name = "",
                Email = "",
                Gender = "",
                Status = ""
            };

            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{URL}/{id}?access-token={ACCESS_TOKEN}"))
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    user = JsonSerializer.Deserialize<UserDTO>(apiResponse, options);
                }
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UserDTO user)
        {
            using (var httpClient = new HttpClient())
            {
                await httpClient.DeleteAsync($"{URL}/{user.Id}?access-token={ACCESS_TOKEN}");
            }

            return RedirectToAction("Index");
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