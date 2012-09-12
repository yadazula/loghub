using LogHub.Web.Models;

namespace LogHub.Web.Infrastructure
{
  public class FakeUserService
  {
    public static User Get(string username, string password)
    {
      if(username == "1" && password == "1")
      {
        return new User {Username = username, Role = UserRole.Administrator};
      }

      return null;
    }
  }
}