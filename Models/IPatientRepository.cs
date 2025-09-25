using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicAppointmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.Models
{
    public interface IPatientRepository
    {
        IEnumerable<Patient> GetAllPatients();
        Patient GetPatientById(int patientId);
        void Add(Patient patient);
        Patient Update(Patient patient);
        void Delete(int patientId);
        bool PatientExists(int patientId);

        // New methods for appointment feature
        Patient GetPatientByEmail(string email);
        bool IsSlotAvailable(int doctorId, DateTime dateTime);
        void BookAppointment(int patientId, int doctorId, DateTime dateTime);
        IEnumerable<Appointment> GetAppointmentsByPatient(int patientId);

        // For doctor
        Task<IEnumerable<Patient>> GetPatientsByDoctorEmailAsync(string doctorEmail);

        // Optional: list all doctors
        IEnumerable<Doctor> GetAllDoctors();



    }
}
