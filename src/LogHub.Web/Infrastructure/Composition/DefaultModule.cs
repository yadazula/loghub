using System.Linq;
using LogHub.Web.Infrastructure.Indexes;
using LogHub.Web.Models;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace LogHub.Web.Infrastructure.Composition
{
  public class DefaultModule : NinjectModule
  {
    public override void Load()
    {
      Bind<IDocumentStore>()
        .ToMethod(_ =>
          {
            var store = new DocumentStore { ConnectionStringName = "RavenDB" }.Initialize();
            store.Conventions.SaveEnumsAsIntegers = true;
            store.Conventions.DefaultQueryingConsistency = ConsistencyOptions.MonotonicRead;

            CreateAdminUserIfNotExist(store);
            IndexCreation.CreateIndexes(typeof(LogMessage_Search).Assembly, store);
            return store;
          })
        .InSingletonScope();

      Bind<IDocumentSession>()
        .ToMethod(x => x.Kernel.Get<IDocumentStore>().OpenSession())
        .InRequestScope();
    }

    private static void CreateAdminUserIfNotExist(IDocumentStore documentStore)
    {
      using (var documentSession = documentStore.OpenSession())
      {
        var admin = documentSession.Query<User>().FirstOrDefault(x => x.Username == "admin");
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