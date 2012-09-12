namespace LogHub.Web.Models
{
  public class User
  {
    public string Username { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }
  }

  public enum UserRole
  {
    None,
    Reader,
    Administrator
  }
}