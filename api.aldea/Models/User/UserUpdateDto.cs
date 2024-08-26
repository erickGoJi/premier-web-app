using System;
using System.ComponentModel.DataAnnotations;

namespace api.premier.Models
{
    public class UserUpdateDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string MotherName { get; set; }
        [Required]
        public string Curp { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required]
        [StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }
        public string Avatar { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public int? DistrictId { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
