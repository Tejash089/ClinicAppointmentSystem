using System;
using System.Collections.Generic;
using System.Linq;
using ClinicAppointmentSystem.Data;
using ClinicAppointmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.Models
{
    public class SQLAppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public SQLAppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all appointments with related Patient and Doctor
        public IEnumerable<Appointment> GetAllAppointments()
        {
            return _context.Appointments
                           .Include(a => a.Patient)
                           .Include(a => a.Doctor)
                           .ToList();
        }

        // Get appointment by ID with related Patient and Doctor
        public Appointment GetAppointment(int appointmentId)
        {
            return _context.Appointments
                           .Include(a => a.Patient)
                           .Include(a => a.Doctor)
                           .FirstOrDefault(a => a.Id == appointmentId);
        }

        // Add new appointment
        public void Add(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            _context.SaveChanges();
        }

        // Update existing appointment
        public Appointment Update(Appointment appointment)
        {
            var existingAppointment = _context.Appointments.Find(appointment.Id);
            if (existingAppointment != null)
            {
                existingAppointment.PatientId = appointment.PatientId;
                existingAppointment.DoctorId = appointment.DoctorId;
                existingAppointment.DateTime = appointment.DateTime;
                existingAppointment.Status = appointment.Status;
                _context.SaveChanges();
            }
            return existingAppointment;
        }

        // Delete appointment
        public void Delete(int appointmentId)
        {
            var appointment = _context.Appointments.Find(appointmentId);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                _context.SaveChanges();
            }
        }

        // Check if appointment exists
        public bool AppointmentExists(int appointmentId)
        {
            return _context.Appointments.Any(a => a.Id == appointmentId);
        }

        public List<Appointment> GetAppointmentsByPatientEmail(string email)
        {
            return _context.Appointments
                           .Where(a => a.Patient.Email == email)
                           .ToList();
        }
    }
}
