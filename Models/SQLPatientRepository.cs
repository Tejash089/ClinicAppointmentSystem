using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinicAppointmentSystem.Data;
using ClinicAppointmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.Models
{
    public class SQLPatientRepository : IPatientRepository
    {
        private readonly AppDbContext _context;

        public SQLPatientRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all patients with their appointments
        public IEnumerable<Patient> GetAllPatients()
        {
            return _context.Patients
                           .Include(p => p.Appointments)
                           .ThenInclude(a => a.Doctor) // optional: include doctor details
                           .ToList();
        }

        // Get patient by ID with their appointments
        public Patient GetPatientById(int patientId)
        {
            return _context.Patients
                           .Include(p => p.Appointments)
                           .ThenInclude(a => a.Doctor) // optional: include doctor details
                           .FirstOrDefault(p => p.Id == patientId);
        }

        // Add new patient
        public void Add(Patient patient)
        {
            _context.Patients.Add(patient);
            _context.SaveChanges();
        }

        // Update existing patient
        public Patient Update(Patient patient)
        {
            var existingPatient = _context.Patients.Find(patient.Id);
            if (existingPatient != null)
            {
                existingPatient.Name = patient.Name;
                existingPatient.Age = patient.Age;
                existingPatient.Gender = patient.Gender;
                existingPatient.Contact = patient.Contact;
                existingPatient.Email = patient.Email;
                _context.SaveChanges();
            }
            return existingPatient;
        }

        // Delete patient
        public void Delete(int patientId)
        {
            var patient = _context.Patients.Find(patientId);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                _context.SaveChanges();
            }
        }

        // Check if patient exists
        public bool PatientExists(int patientId)
        {
            return _context.Patients.Any(p => p.Id == patientId);
        }

        // --- Appointment feature ---
        public Patient GetPatientByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;  // don’t query if no email from Identity

            return _context.Patients
                           .Include(p => p.Appointments)
                           .ThenInclude(a => a.Doctor)
                           .FirstOrDefault(p => p.Email.ToLower() == email.ToLower());
        }




        public bool IsSlotAvailable(int doctorId, DateTime dateTime)
        {
            return !_context.Appointments.Any(a => a.DoctorId == doctorId && a.DateTime == dateTime);
        }

        
        public void BookAppointment(int patientId, int doctorId, DateTime dateTime)
        {
            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = doctorId,
                DateTime = dateTime,
                Status = "Scheduled"
            };
            _context.Appointments.Add(appointment);

            var rows = _context.SaveChanges();
            Console.WriteLine("BookAppointment Debug - Rows affected: " + rows);
        }

        public IEnumerable<Appointment> GetAppointmentsByPatient(int patientId)
        {
            return _context.Appointments
                           .Include(a => a.Doctor)
                           .Where(a => a.PatientId == patientId)
                           .ToList();
        }

        public async Task<IEnumerable<Patient>> GetPatientsByDoctorEmailAsync(string doctorEmail)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Email == doctorEmail);
            if (doctor == null) return new List<Patient>();

            return await _context.Appointments
                                 .Where(a => a.DoctorId == doctor.Id)
                                 .Select(a => a.Patient)
                                 .Distinct()
                                 .ToListAsync();
        }

        public IEnumerable<Doctor> GetAllDoctors()
        {
            return _context.Doctors.ToList();
        }




    }
}
