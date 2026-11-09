using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Controllers
{
    public class OrganiserEnrolmentsController : Controller
    {
        private readonly ApiService _api;

        public OrganiserEnrolmentsController(ApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index(int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            var enrolments = await _api.GetAsync<List<OrganiserEnrolmentViewModel>>(
                $"api/enrolments/event/{eventId}");

            ViewBag.EventId = eventId;

            return View(enrolments ?? new());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int id, int eventId, string status)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            var request = new { Status = status };

            await _api.SendAsync(
                HttpMethod.Put,
                $"api/enrolments/{id}/status",
                JsonContent.Create(request));

            return RedirectToAction("Index", new { eventId });
        }
    }
}