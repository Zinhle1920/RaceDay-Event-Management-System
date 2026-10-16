
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.Models;

public class AppUser
{
    [Key]
    public int UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string Role { get; set; } = "Participant";
}
