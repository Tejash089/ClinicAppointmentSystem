using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClinicAppointmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointmentSystem.Controllers
{
    [Authorize(Roles = "Admin,Doctor")] // Both can view
    public class DoctorsController : Controller
    {
        private readonly IDoctorRepository _doctorRepo;

        public DoctorsController(IDoctorRepository doctorRepo)
        {
            _doctorRepo = doctorRepo;
        }

        // List all doctors
        public IActionResult Index()
        {
            var model = _doctorRepo.GetAllDoctors();
            return View(model);
        }

        // Show details of a doctor
        public IActionResult Details(int id)
        {
            var doctor = _doctorRepo.GetDoctorById(id);
            if (doctor == null)
            {
                Response.StatusCode = 404;
                return View("DoctorNotFound", id);
            }
            return View(doctor);
        }

        [Authorize(Roles = "Admin")]
        // GET: Create new doctor
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: Create new doctor
        [HttpPost]
        public IActionResult Create(Doctor model)
        {
            if (ModelState.IsValid)
            {
                _doctorRepo.Add(model);
                return RedirectToAction("Details", new { id = model.Id });
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        // GET: Edit doctor
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var doctor = _doctorRepo.GetDoctorById(id);
            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        [Authorize(Roles = "Admin")]
        // POST: Save edits
        [HttpPost]
        public IActionResult Edit(Doctor model)
        {
            if (ModelState.IsValid)
            {
                _doctorRepo.Update(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        // GET: Delete confirmation
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var doctor = _doctorRepo.GetDoctorById(id);
            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        [Authorize(Roles = "Admin")]
        // POST: Confirm delete
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _doctorRepo.Delete(id);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Doctor")]
        public IActionResult MyAppointments()
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity.Name;
            Console.WriteLine("Doctor MyAppointments Debug - Logged-in email: " + (email ?? "NULL"));

            var doctor = _doctorRepo.GetDoctorByEmail(email);
            Console.WriteLine("Doctor MyAppointments Debug - Found doctor: " + (doctor?.Name ?? "NULL"));

            if (doctor == null)
            {
                TempData["Error"] = "No doctor record found for your email.";
                return View(new List<Appointment>());
            }

            var appointments = _doctorRepo.GetAppointmentsByDoctor(doctor.Id);
            Console.WriteLine("Doctor MyAppointments Debug - Found appointments count: " + appointments.Count());

            return View(appointments);
        }



        [Authorize(Roles = "Doctor")]
        public IActionResult MyPatients()
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity.Name;
            Console.WriteLine("Doctor MyPatients Debug - Logged-in email: " + (email ?? "NULL"));

            var doctor = _doctorRepo.GetDoctorByEmail(email);
            Console.WriteLine("Doctor MyPatients Debug - Found doctor: " + (doctor?.Name ?? "NULL"));

            if (doctor == null)
            {
                TempData["Error"] = "No doctor record found for your email.";
                return View(new List<Patient>());
            }

            var patients = _doctorRepo.GetPatientsByDoctorEmailAsync(email).Result.ToList();
            Console.WriteLine("Doctor MyPatients Debug - Found patients count: " + patients.Count());

            return View(patients);
        }






    }
}
