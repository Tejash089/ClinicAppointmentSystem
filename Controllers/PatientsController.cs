using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ClinicAppointmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.Controllers
{
    [Authorize] // Require login for all actions
    public class PatientsController : Controller
    {
        private readonly IPatientRepository _patientRepo;

        public PatientsController(IPatientRepository patientRepo)
        {
            _patientRepo = patientRepo;
        }

        [Authorize(Roles = "Admin,Patient")]
        // List all patients
        public IActionResult Index()
        {
            var model = _patientRepo.GetAllPatients();
            return View(model);
        }

        [Authorize(Roles = "Admin,Patient")]
        // Show details of a patient, including their appointments
        public IActionResult Details(int id)
        {
            var patient = _patientRepo.GetPatientById(id);
            if (patient == null)
            {
                Response.StatusCode = 404;
                return View("PatientNotFound", id);
            }
            return View(patient);
        }

        [Authorize(Roles = "Admin")]
        // GET: Show form to create a new patient
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: Create new patient
        [HttpPost]
        public IActionResult Create(Patient model)
        {
            if (ModelState.IsValid)
            {
                _patientRepo.Add(model);
                return RedirectToAction("Details", new { id = model.Id });
            }
            return View();
        }

        [Authorize(Roles = "Admin,Patient")]
        // GET: Show edit form
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var patient = _patientRepo.GetPatientById(id);
            if (patient == null)
                return NotFound();

            return View(patient);
        }

        [Authorize(Roles = "Admin,Patient")]
        // POST: Save patient edits
        [HttpPost]
        public IActionResult Edit(Patient model)
        {
            if (ModelState.IsValid)
            {
                _patientRepo.Update(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        // GET: Show delete confirmation
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var patient = _patientRepo.GetPatientById(id);
            if (patient == null)
                return NotFound();

            return View(patient);
        }

        [Authorize(Roles = "Admin")]
        // POST: Confirm deletion
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _patientRepo.Delete(id);
            return RedirectToAction("Index");
        }

        // --- Book Appointment ---
        [Authorize(Roles = "Patient")]
        [HttpGet]
        public IActionResult BookAppointment()
        {
            ViewBag.Doctors = _patientRepo.GetAllDoctors();
            return View();
        }

        
        [Authorize(Roles = "Patient")]
        
        [HttpPost]
        public IActionResult BookAppointment(int doctorId, DateTime dateTime)
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity.Name;
            Console.WriteLine("Debug - Logged-in email: " + email);


            var patient = _patientRepo.GetPatientByEmail(email);
            Console.WriteLine("BookAppointment Debug - Found patient: " + (patient?.Name ?? "NULL"));

            if (patient == null)
            {
                TempData["Error"] = "No patient record found for your email.";
                ViewBag.Doctors = _patientRepo.GetAllDoctors();
                return View();
            }

            Console.WriteLine("Checking slot for DoctorId: " + doctorId + " at " + dateTime);

            if (!_patientRepo.IsSlotAvailable(doctorId, dateTime))
            {
                TempData["Error"] = "This slot is already booked!";
                ViewBag.Doctors = _patientRepo.GetAllDoctors();
                return View();
            }

            _patientRepo.BookAppointment(patient.Id, doctorId, dateTime);
            Console.WriteLine("BookAppointment Debug - Appointment saved for patientId: " + patient.Id);

            TempData["Success"] = "Appointment booked successfully!";
            return RedirectToAction("MyAppointments");
        }

        [Authorize(Roles = "Patient")]
        public IActionResult MyAppointments()
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity.Name;
            Console.WriteLine("MyAppointments Debug - Logged-in email: " + (email ?? "NULL"));

            var patient = _patientRepo.GetPatientByEmail(email);
            Console.WriteLine("MyAppointments Debug - Found patient: " + (patient?.Name ?? "NULL"));

            if (patient == null)
            {
                TempData["Error"] = "No patient record found for your email.";
                return View(new List<Appointment>());
            }

            var appointments = _patientRepo.GetAppointmentsByPatient(patient.Id);
            Console.WriteLine("MyAppointments Debug - Found appointments count: " + appointments.Count());

            return View(appointments);
        }



        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> MyPatients()
        {
            var doctorEmail = User.FindFirstValue(ClaimTypes.Email);
            var patients = await _patientRepo.GetPatientsByDoctorEmailAsync(doctorEmail);
            return View(patients);
        }



    }
}
