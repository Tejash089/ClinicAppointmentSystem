using System.ComponentModel.DataAnnotations;

namespace ClinicAppointmentSystem.Models
{
    public enum UserRole
    {
        Admin,
        Doctor,
        Patient
    }

    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public UserRole Role { get; set; }
    }
}
