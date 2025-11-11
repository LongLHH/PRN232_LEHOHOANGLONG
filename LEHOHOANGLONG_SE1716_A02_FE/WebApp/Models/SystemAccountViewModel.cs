using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class SystemAccountViewModel
{
    public short AccountId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100)]
    public string AccountName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100)]
    public string AccountEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required")]
    public int AccountRole { get; set; }

    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string? AccountPassword { get; set; }

    public string RoleName => AccountRole switch
    {
        0 => "Admin",
        1 => "Staff",
        2 => "Lecturer",
        _ => "Unknown"
    };
}
