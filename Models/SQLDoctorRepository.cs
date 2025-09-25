using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinicAppointmentSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicAppointmentSystem.Models
{
    public class SQLDoctorRepository : IDoctorRepository
    {
        private readonly AppDbContext _context;

        public SQLDoctorRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all doctors with their appointments
        public IEnumerable<Doctor> GetAllDoctors()
        {
            return _context.Doctors
                           .Include(d => d.Appointments)
                           .ThenInclude(a => a.Patient) // optional: include patient details
                           .ToList();
        }

        // Get doctor by ID with appointments
        public Doctor GetDoctorById(int doctorId)
        {
            return _context.Doctors
                           .Include(d => d.Appointments)
                           .ThenInclude(a => a.Patient) // optional
                           .FirstOrDefault(d => d.Id == doctorId);
        }

        // Add new doctor
        public void Add(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            _context.SaveChanges();
        }

        // Update existing doctor
        public Doctor Update(Doctor doctor)
        {
            var existingDoctor = _context.Doctors.Find(doctor.Id);
            if (existingDoctor != null)
            {
                existingDoctor.Name = doctor.Name;
                existingDoctor.Specialization = doctor.Specialization;
                existingDoctor.Contact = doctor.Contact;
                existingDoctor.Email = doctor.Email;
                _context.SaveChanges();
            }
            return existingDoctor;
        }

        // Delete doctor
        public void Delete(int doctorId)
        {
            var doctor = _context.Doctors.Find(doctorId);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
                _context.SaveChanges();
            }
        }

        // Check if doctor exists
        public bool DoctorExists(int doctorId)
        {
            return _context.Doctors.Any(d => d.Id == doctorId);
        }


        public Doctor GetDoctorByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            return _context.Doctors
                           .Include(d => d.Appointments)
                           .ThenInclude(a => a.Patient)
                           .FirstOrDefault(d => d.Email.ToLower() == email.ToLower());
        }

        public IEnumerable<Appointment> GetAppointmentsByDoctor(int doctorId)
        {
            return _context.Appointments
                           .Include(a => a.Patient)
                           .Where(a => a.DoctorId == doctorId)
                           .ToList();
        }
       


        
        public async Task<IEnumerable<Patient>> GetPatientsByDoctorEmailAsync(string doctorEmail)
        {
            var doctor = await _context.Doctors
                                       .FirstOrDefaultAsync(d => d.Email.ToLower() == doctorEmail.ToLower());

            if (doctor == null) return new List<Patient>();

            return await _context.Appointments
                                 .Where(a => a.DoctorId == doctor.Id)
                                 .Select(a => a.Patient)
                                 .Distinct()
                                 .ToListAsync();
        }


    }
}
