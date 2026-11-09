using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Controllers
{
    public class EnrolmentsController : Controller
    {
        private readonly ApiService _api;

        public EnrolmentsController(ApiService api)
        {
            _api = api;
        }

        private bool IsParticipant()
        {
            return HttpContext.Session.GetString("Role") == "Participant";
        }

        [HttpGet]
        public async Task<IActionResult> Create(int eventId)
        {
            if (!IsParticipant())
                return RedirectToAction("Login", "Auth");

            var eventDetails = await _api.GetAsync<EventViewModel>(
                $"api/events/{eventId}");

            if (eventDetails == null)
                return NotFound();

            var categories = await _api.GetAsync<List<CategoryViewModel>>(
                $"api/categories/event/{eventId}");

            var model = new EnrolmentViewModel
            {
                EventId = eventId,
                EventName = eventDetails.EventName,
                Categories = categories ?? new()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EnrolmentViewModel model)
        {
            if (!IsParticipant())
                return RedirectToAction("Login", "Auth");

            var eventDetails = await _api.GetAsync<EventViewModel>(
                $"api/events/{model.EventId}");

            if (eventDetails == null)
                return NotFound();

            model.EventName = eventDetails.EventName;
            model.Categories = await _api.GetAsync<List<CategoryViewModel>>(
                $"api/categories/event/{model.EventId}") ?? new();

            if (!ModelState.IsValid)
                return View(model);

            var request = new
            {
                model.EventId,
                model.CategoryId
            };

            var response = await _api.SendAsync(
                HttpMethod.Post,
                "api/enrolments",
                JsonContent.Create(request));

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("",
                    "Unable to register. You may already be enrolled or the category may be invalid.");

                return View(model);
            }

            TempData["SuccessMessage"] = "Your event registration was successful!";

            return RedirectToAction("Index", "Participant");
        }
    }
}