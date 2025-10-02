using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicAppointmentSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full Name must be between 2 and 100 characters")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

       

        [StringLength(200)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public override string PhoneNumber { get; set; }

        // Password is handled by Identity; no need to add it here
    }
}
