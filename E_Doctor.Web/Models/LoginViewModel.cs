using System.ComponentModel.DataAnnotations;

namespace E_Doctor.Admin.Models;
public class LoginViewModel
{
    [Required]
    public string? Username { get; set; }

    [Required]
    public string? Password { get; set; }
}
