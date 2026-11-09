using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }

        [Required]
        public string FirstName { get; set; } = "";

        [Required]
        public string LastName { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        public string Phone { get; set; } = "";

        public string Role { get; set; } = "";
    }
}