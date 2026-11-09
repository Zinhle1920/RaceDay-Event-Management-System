using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    public class MyEnrolmentsController : Controller
    {
        private readonly ApiService _api;

        public MyEnrolmentsController(ApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Login", "Auth");

            var enrolments =
                await _api.GetAsync<List<ParticipantEnrolmentViewModel>>(
                    "api/enrolments/mine");

            return View(enrolments ?? new());
        }
    }
}