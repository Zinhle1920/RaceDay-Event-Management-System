using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApiService _api;

        public CategoriesController(ApiService api)
        {
            _api = api;
        }

        private bool IsOrganiser()
        {
            return HttpContext.Session.GetString("Role") == "Organiser";
        }

        // CREATE CATEGORY

        [HttpGet]
        public IActionResult Create(int eventId)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            return View(new CategoryFormViewModel
            {
                EventId = eventId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            var request = new
            {
                model.EventId,
                model.CategoryName,
                model.Description
            };

            var response = await _api.SendAsync(
                HttpMethod.Post,
                "api/categories",
                JsonContent.Create(request));

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Unable to create category.");
                return View(model);
            }

            return RedirectToAction("Details", "Events",
                new { id = model.EventId });
        }

        // EDIT CATEGORY

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int eventId)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            var categories = await _api.GetAsync<List<CategoryViewModel>>(
                $"api/categories/event/{eventId}");

            var category = categories?.FirstOrDefault(c => c.CategoryId == id);

            if (category == null)
                return NotFound();

            var model = new CategoryFormViewModel
            {
                CategoryId = category.CategoryId,
                EventId = category.EventId,
                CategoryName = category.CategoryName,
                Description = category.Description
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryFormViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            var request = new
            {
                model.EventId,
                model.CategoryName,
                model.Description
            };

            var response = await _api.SendAsync(
                HttpMethod.Put,
                $"api/categories/{model.CategoryId}",
                JsonContent.Create(request));

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Unable to update category.");
                return View(model);
            }

            return RedirectToAction("Details", "Events",
                new { id = model.EventId });
        }

        // DELETE CATEGORY

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int eventId)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            await _api.SendAsync(
                HttpMethod.Delete,
                $"api/categories/{id}");

            return RedirectToAction("Details", "Events",
                new { id = eventId });
        }
    }
}