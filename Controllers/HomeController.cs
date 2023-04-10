using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication2.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
//using PagedList;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataAccessLayer.Entities;
using AutoMapper;

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

        private readonly List<SelectListItem> genders = new List<SelectListItem>
            {
                new SelectListItem {Text = Gender.Male.ToString(), Value = ((int)Gender.Male).ToString()},
                new SelectListItem {Text = Gender.Female.ToString(), Value = ((int)Gender.Female).ToString()}
            };

        private readonly List<SelectListItem> status = new List<SelectListItem>
            {
                new SelectListItem {Text = Status.Active.ToString(), Value = Status.Active.ToString()},
                new SelectListItem {Text = Status.Inactive.ToString(), Value = Status.Inactive.ToString()}
            };

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<User> usersList = new List<User>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync($"{URL}?access-token={ACCESS_TOKEN}"))
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    usersList = JsonSerializer.Deserialize<List<User>>(apiResponse, options);
                }
            }

            return View(usersList);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Genders = new SelectList(genders, "Value", "Text");

            ViewBag.Status = new SelectList(status, "Value", "Text");

            var user = new UserDTO
            {
                Name = "",
                Email = "",
                Gender = null,
                Status = null
            };

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserDTO user)
        {
            User _user = new User { 
                Name = user.Name,
                Email = user.Email,
                Gender = user.Gender?.ToString(),
                Status = user.Status?.ToString()
            };

            using (var httpClient = new HttpClient())
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = new ToLowerNamingPolicy()
                };
                var json = JsonSerializer.Serialize<User>(_user, options).ToString();
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await httpClient.PostAsync($"{URL}?access-token={ACCESS_TOKEN}", content);

            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            //if(id == null) 
            var user = new User
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
                    user = JsonSerializer.Deserialize<User>(apiResponse, options);
                }
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(User user)
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