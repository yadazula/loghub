using System.ComponentModel.DataAnnotations;
using LogHub.Web.Models;

namespace LogHub.Web.ViewModels
{
  public class UserInput
  {
    [Required]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Name { get; set; }

    public UserRole Role { get; set; }

    public string Password { get; set; }
  }
}