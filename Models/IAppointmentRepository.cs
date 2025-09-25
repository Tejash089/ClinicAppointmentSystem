using System;
using System.Collections.Generic;
using ClinicAppointmentSystem.Migrations;

namespace ClinicAppointmentSystem.Models
{
    public interface IAppointmentRepository
    {   
        Appointment GetAppointment(int id);
        IEnumerable<Appointment> GetAllAppointments();
        void Add(Appointment appointment);
        Appointment Update(Appointment appointmentChanges);
        void Delete(int id);
        bool AppointmentExists(int id);
        List<Appointment> GetAppointmentsByPatientEmail(string email);



    }
}
