using System.Linq;
using LogHub.Web.Models;
using Raven.Client;

namespace LogHub.Web.Infrastructure.Modules.Tasks
{
  public class CheckIfAdminUserExist : IStartupTask
  {
    private readonly IDocumentStore documentStore;

    public CheckIfAdminUserExist(IDocumentStore documentStore)
    {
      this.documentStore = documentStore;
    }

    public void Execute()
    {
      using (var documentSession = documentStore.OpenSession())
      {
        var admin = documentSession.Query<User>()
                                   .FirstOrDefault(x => x.Username == "admin");

        if (admin == null)
        {
          var user = new User { Username = "admin", Name = "Administrator", Role = UserRole.Administrator };
          user.SetPassword("loghub");

          documentSession.Store(user);
          documentSession.SaveChanges();
        }
      }      
    }
  }
}