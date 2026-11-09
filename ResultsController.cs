using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Controllers
{
    public class ResultsController : Controller
    {
        private readonly ApiService _api;

        public ResultsController(ApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Login", "Auth");

            var results = await _api.GetAsync<List<ResultViewModel>>(
                "api/results/mine");

            return View(results ?? new());
        }

        [HttpGet]
        public IActionResult Create(int enrolmentId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            return View(new CreateResultViewModel
            {
                EnrolmentId = enrolmentId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateResultViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(model);

            var request = new
            {
                model.EnrolmentId,
                model.FinishTime,
                model.FinishPosition
            };

            var response = await _api.SendAsync(
                HttpMethod.Post,
                "api/results",
                JsonContent.Create(request));

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Unable to save result.");
                return View(model);
            }

            return RedirectToAction("Index", "Organiser");
        }
    }
}