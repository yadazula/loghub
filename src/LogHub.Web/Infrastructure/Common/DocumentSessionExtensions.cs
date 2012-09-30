using System.Linq;
using LogHub.Core.Models;
using Raven.Client;

namespace LogHub.Web.Infrastructure.Common
{
  public static class DocumentSessionExtensions
  {
    public static User GetUserByUsername(this IDocumentSession documentSession, string username)
    {
      var user = documentSession.Query<User>().SingleOrDefault(x => x.Username == username);
      return user;
    }
  }
}