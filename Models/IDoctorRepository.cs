using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicAppointmentSystem.Models
{
    public interface IDoctorRepository
    {
        IEnumerable<Doctor> GetAllDoctors();
        Doctor GetDoctorById(int doctorId);
        void Add(Doctor doctor);
        Doctor Update(Doctor doctor);
        void Delete(int doctorId);
        bool DoctorExists(int doctorId);


        Doctor GetDoctorByEmail(string email);

        IEnumerable<Appointment> GetAppointmentsByDoctor(int doctorId);
        Task<IEnumerable<Patient>> GetPatientsByDoctorEmailAsync(string doctorEmail);

    }
}
