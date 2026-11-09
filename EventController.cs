using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApiService _api;

        public EventsController(ApiService api)
        {
            _api = api;
        }

        private bool IsOrganiser()
        {
            return HttpContext.Session.GetString("Role") == "Organiser";
        }

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        // CREATE EVENT

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            var model = new EventFormViewModel
            {
                EventTypes = await _api.GetAsync<List<EventTypeViewModel>>(
                    "api/eventtypes") ?? new()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventFormViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            model.EventTypes = await _api.GetAsync<List<EventTypeViewModel>>(
                "api/eventtypes") ?? new();

            if (!ModelState.IsValid)
                return View(model);

            var request = new
            {
                model.EventName,
                model.Description,
                model.EventDate,
                model.Location,
                model.Distance,
                model.EventTypeId
            };

            var response = await _api.SendAsync(
                HttpMethod.Post,
                "api/events",
                JsonContent.Create(request));

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Unable to create event.");
                return View(model);
            }

            return RedirectToAction("Index", "Organiser");
        }

        // EVENT DETAILS

        public async Task<IActionResult> Details(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Auth");

            var eventDetails = await _api.GetAsync<EventViewModel>(
                $"api/events/{id}");

            if (eventDetails == null)
                return NotFound();

            var categories = await _api.GetAsync<List<CategoryViewModel>>(
                $"api/categories/event/{id}");

            ViewBag.Categories = categories ?? new List<CategoryViewModel>();

            return View(eventDetails);
        }

        // EDIT EVENT

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            var eventDetails = await _api.GetAsync<EventViewModel>(
                $"api/events/{id}");

            if (eventDetails == null)
                return NotFound();

            var model = new EventFormViewModel
            {
                EventName = eventDetails.EventName,
                Description = eventDetails.Description,
                EventDate = eventDetails.EventDate,
                Location = eventDetails.Location,
                Distance = eventDetails.Distance,
                EventTypeId = eventDetails.EventTypeId,
                EventTypes = await _api.GetAsync<List<EventTypeViewModel>>(
                    "api/eventtypes") ?? new()
            };

            ViewBag.EventId = id;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventFormViewModel model)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            model.EventTypes = await _api.GetAsync<List<EventTypeViewModel>>(
                "api/eventtypes") ?? new();

            if (!ModelState.IsValid)
            {
                ViewBag.EventId = id;
                return View(model);
            }

            var request = new
            {
                model.EventName,
                model.Description,
                model.EventDate,
                model.Location,
                model.Distance,
                model.EventTypeId
            };

            var response = await _api.SendAsync(
                HttpMethod.Put,
                $"api/events/{id}",
                JsonContent.Create(request));

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Unable to update event.");
                ViewBag.EventId = id;
                return View(model);
            }

            return RedirectToAction("Index", "Organiser");
        }

        // DELETE EVENT

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsOrganiser())
                return RedirectToAction("Login", "Auth");

            await _api.SendAsync(
                HttpMethod.Delete,
                $"api/events/{id}");

            return RedirectToAction("Index", "Organiser");
        }
    }
}