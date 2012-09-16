using System;
using System.Security.Cryptography;
using System.Text;

namespace LogHub.Web.Models
{
  public class User
  {
    const string ConstantSalt = "!Ogw5Z*26xW#R";
    public const string UndeletableAdminUser = "admin";

    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }

    protected string HashedPassword { get; private set; }
    private string passwordSalt;
    private string PasswordSalt
    {
      get
      {
        return passwordSalt ?? (passwordSalt = Guid.NewGuid().ToString("N"));
      }
      set { passwordSalt = value; }
    }

    public void SetPassword(string password)
    {
      HashedPassword = GetHashedPassword(password);
    }

    public bool ValidatePassword(string maybePassword)
    {
      if (HashedPassword == null)
        return true;
      return HashedPassword == GetHashedPassword(maybePassword);
    }

    private string GetHashedPassword(string password)
    {
      using (var sha = SHA256.Create())
      {
        var computedHash = sha.ComputeHash(Encoding.Unicode.GetBytes(PasswordSalt + password + ConstantSalt));
        return Convert.ToBase64String(computedHash);
      }
    }
  }
}