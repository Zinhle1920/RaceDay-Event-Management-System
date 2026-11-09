using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly ApiService _api;

        public OrganiserController(ApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            var events = await _api.GetAsync<List<EventViewModel>>(
                "api/events/mine") ?? new List<EventViewModel>();

            var model = new DashboardViewModel
            {
                FirstName = HttpContext.Session.GetString("FirstName") ?? "",
                Events = events
            };

            return View(model);
        }
    }
}