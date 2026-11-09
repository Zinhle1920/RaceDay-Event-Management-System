using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApiService _api;

        public ProfileController(ApiService api)
        {
            _api = api;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");

            var profile = await _api.GetAsync<ProfileViewModel>(
                "api/profile/me");

            if (profile == null)
                return RedirectToAction("Login", "Auth");

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            var request = new
            {
                model.FirstName,
                model.LastName,
                model.Phone
            };

            var response = await _api.SendAsync(
                HttpMethod.Put,
                "api/profile/me",
                JsonContent.Create(request));

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Unable to update profile.");
                return View(model);
            }

            HttpContext.Session.SetString("FirstName", model.FirstName);

            TempData["SuccessMessage"] = "Profile updated successfully.";

            return RedirectToAction("Index");
        }
    }
}