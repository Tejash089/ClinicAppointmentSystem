using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicAppointmentSystem.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Patient")]
        public int PatientId { get; set; }

        [Required]
        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } // Scheduled, Completed, Cancelled

        // Navigation properties
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
    }
}
