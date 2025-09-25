using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ClinicAppointmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ClinicAppointmentSystem.Controllers
{
    [Authorize] // Require login for all actions in this controller
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Automatically redirect to dashboard based on role
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("AdminDashboard");
            else if (User.IsInRole("Doctor"))
                return RedirectToAction("DoctorDashboard");
            else if (User.IsInRole("Patient"))
                return RedirectToAction("PatientDashboard");

            // fallback: if somehow user has no role
            return RedirectToAction("Privacy");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }

        [Authorize(Roles = "Doctor")]
        public IActionResult DoctorDashboard()
        {
            return View();
        }

        [Authorize(Roles = "Patient")]
        public IActionResult PatientDashboard()
        {
            return View();
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
