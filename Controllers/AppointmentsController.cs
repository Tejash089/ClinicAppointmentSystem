using ClinicAppointmentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicAppointmentSystem.Controllers
{
    [Authorize] // All actions require login
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IPatientRepository _patientRepo;
        private readonly IDoctorRepository _doctorRepo;

        public AppointmentsController(IAppointmentRepository appointmentRepo,
                                      IPatientRepository patientRepo,
                                      IDoctorRepository doctorRepo)
        {
            _appointmentRepo = appointmentRepo;
            _patientRepo = patientRepo;
            _doctorRepo = doctorRepo;
        }

        // GET: All appointments
        public IActionResult Index()
        {
            var model = _appointmentRepo.GetAllAppointments();
            return View(model);
        }

        // GET: Appointment details
        public IActionResult Details(int id)
        {
            var appointment = _appointmentRepo.GetAppointment(id);
            if (appointment == null)
            {
                Response.StatusCode = 404;
                return View("AppointmentNotFound", id);
            }
            return View(appointment);
        }

        [Authorize(Roles = "Admin,Patient")] // Only Admin and Patient can create
        // GET: Create appointment
        [HttpGet]
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        [Authorize(Roles = "Admin,Patient")] // Only Admin and Patient can create
        // POST: Create appointment
        [HttpPost]
        public IActionResult Create(Appointment model)
        {
            if (ModelState.IsValid)
            {
                _appointmentRepo.Add(model);
                return RedirectToAction("Details", new { id = model.Id });
            }

            LoadDropdowns(model.PatientId, model.DoctorId);
            return View(model);
        }

        [Authorize(Roles = "Admin,Doctor")]
        // GET: Edit appointment
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var appointment = _appointmentRepo.GetAppointment(id);
            if (appointment == null)
                return NotFound();

            LoadDropdowns(appointment.PatientId, appointment.DoctorId);
            return View(appointment);
        }

        [Authorize(Roles = "Admin,Doctor")]
        // POST: Edit appointment
        [HttpPost]
        public IActionResult Edit(Appointment model)
        {
            if (ModelState.IsValid)
            {
                _appointmentRepo.Update(model);
                return RedirectToAction("Index");
            }

            LoadDropdowns(model.PatientId, model.DoctorId);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        // GET: Delete appointment
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var appointment = _appointmentRepo.GetAppointment(id);
            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        [Authorize(Roles = "Admin")]
        // POST: Confirm delete
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _appointmentRepo.Delete(id);
            return RedirectToAction("Index");
        }

        // 🔹 Helper method for dropdowns
        private void LoadDropdowns(int? selectedPatientId = null, int? selectedDoctorId = null)
        {
            var patients = _patientRepo.GetAllPatients();
            var doctors = _doctorRepo.GetAllDoctors();

            ViewBag.PatientId = new SelectList(patients, "Id", "Name", selectedPatientId);
            ViewBag.DoctorId = new SelectList(doctors, "Id", "Name", selectedDoctorId);

        }
    }
}
